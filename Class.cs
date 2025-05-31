using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WeightedRandomSelection;
namespace RandPick {



    public class Class {
        public enum SetStudentSomethingFromFileResult {

            /// <summary>
            /// The success
            /// </summary>
            Success,
            /// <summary>
            /// 未找到文件
            /// </summary>
            FileNotFound,
            /// <summary>
            /// 文件格式无效
            /// </summary>
            InvalidFileFormat, 
            /// <summary>
            /// 其他未知错误
            /// </summary>
            UnknownError   
        }
        public enum SelectStudentNamesResult {
            /// <summary>
            /// 成功
            /// </summary>
            Success,
            /// <summary>
            /// 没有学生姓名
            /// </summary>
            NoStudentNames, 
            /// <summary>
            /// 未知错误
            /// </summary>
            UnknownError
        }
        private readonly IntRangeWeightedRandomSelector _studentWeightedRandomSelector;
        //private readonly List<int> _studentIds;
        private readonly List<string> _studentNames = [];
        //private List<double> _studentWeights;
        public bool HasStudentName { get; set; }
        //public string Name { get; set; }
        public int StudentCount { get; set; }


        public Class(int studentCount, string? studentNamesFileName = null) {
            _studentWeightedRandomSelector = new IntRangeWeightedRandomSelector(1, studentCount);
            //_studentIds = new List<int>();
            //_studentIds = Enumerable.Range(start: 1, count: studentCount).ToList();
            StudentCount = studentCount;
            //_studentWeights = Enumerable.Repeat(1.0, studentCount).ToList();
            if (studentNamesFileName == null) {
                HasStudentName = false;
            }
            else {
                if (SetStudentNameFromFile(ref _studentNames, studentNamesFileName) == SetStudentSomethingFromFileResult.Success) {
                    HasStudentName = true;
                }
                else {
                    HasStudentName = false;
                }
            }
        }


        /// <summary>
        /// 从文件设置学生姓名列表.
        /// </summary>
        /// <param name="studentNames">学生姓名列表</param>
        /// <param name="fileName">存储学生姓名的文件的文件名.</param>
        /// <returns></returns>
        private static SetStudentSomethingFromFileResult SetStudentNameFromFile(ref List<string> studentNames, string fileName) {

            if (File.Exists(fileName)) {
                string[] allLines = File.ReadAllLines(fileName);
                int lineNumber = 0;//from 0
                foreach (string line in allLines) {
                    studentNames[lineNumber] = line;
                    lineNumber++;
                }
                return SetStudentSomethingFromFileResult.Success;
            }
            else {
                return SetStudentSomethingFromFileResult.FileNotFound;
            }
        }

        /// <summary>
        /// 设置学号权重.
        /// </summary>
        /// <param name="id">学号.</param>
        /// <param name="weight">权重.</param>
        public void AddOrUpdateStudentWeight(int id, double weight) {
            _studentWeightedRandomSelector.AddOrUpdateWeight(id, weight);
        }


        /// <summary>
        /// 从文件设置学号权重.
        /// </summary>
        /// <param name="fileName">存储学号权重的文件的文件名.</param>
        /// <returns>成功true失败false</returns>
        public SetStudentSomethingFromFileResult AddOrUpdateStudentWeightFromFile(string fileName) {
            if (File.Exists(fileName) == true) {
                string[] allLines = File.ReadAllLines(fileName);
                //int Number;
                //double Weight;
                foreach (string line in allLines) {
                    //Number = line.Substring(0, 2);
                    //WeightString = line.Substring(3).;
                    try {
                        if (int.TryParse(line.AsSpan(0, 2), out int Number) && double.TryParse(line.AsSpan(3), out double Weight)) {
                            _studentWeightedRandomSelector.AddOrUpdateWeight(Number, Weight);
                            return SetStudentSomethingFromFileResult.Success;
                        }
                        else {
                            return SetStudentSomethingFromFileResult.InvalidFileFormat;
                        }
                    }
                    catch {
                        return SetStudentSomethingFromFileResult.InvalidFileFormat;
                    }
                }
                return SetStudentSomethingFromFileResult.Success;
            }
            else {
                return SetStudentSomethingFromFileResult.FileNotFound;
            }
        }

        /// <summary>
        /// 抽取学生学号.
        /// </summary>
        /// <param name="count">抽取数量.</param>
        /// <returns>抽取的学号集合</returns>
        public IEnumerable<int> SelectMultipleStudentIds(int count) {
            return _studentWeightedRandomSelector.SelectMultiple(count).Order();
        }

        /// <summary>
        /// 抽取学生名字.
        /// </summary>
        /// <param name="count">抽取数量.</param>
        /// <returns>抽取的学生姓名集合</returns>
        /// <exception cref="System.InvalidOperationException">The class doesn't have student names</exception>
        public SelectStudentNamesResult SelectMultipleStudentNames(int count,out List<string> selectStudent) {
            if (HasStudentName == false) {
                selectStudent = [];
                return SelectStudentNamesResult.NoStudentNames;
            }
            else {
                List<int> selectedStudentIds = [.. _studentWeightedRandomSelector.SelectMultiple(count).Order()];
                selectStudent = [];
                foreach (int id in selectedStudentIds) {
                    selectStudent.Add(id.ToString("00") + _studentNames[id - 1]);
                }
                return SelectStudentNamesResult.Success;
            }
        }

    }
}
