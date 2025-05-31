using System;
using System.Collections.Generic;
using System.Linq;

namespace WeightedRandomSelection {
    /// <summary>
    /// 带权重的随机选择器
    /// </summary>
    /// <typeparam name="T">要选择的项类型</typeparam>
    public class WeightedRandomSelector<T> where T : notnull {
        private readonly Random _random = new();
        private readonly Dictionary<T, double> _weights = [];
        private readonly List<T> _items = [];
        private readonly List<double> _cumulativeWeights = [];
        private double _totalWeight;
        private bool _isDirty = true;

        /// <summary>
        /// 添加或更新项的权重
        /// </summary>
        /// <param name="item">要添加的项</param>
        /// <param name="weight">权重值(必须为正数)</param>
        public void AddOrUpdateWeight(T item, double weight) {
            if (weight <= 0)
                throw new ArgumentOutOfRangeException(nameof(weight), "Weight must be positive");

            if (!_weights.TryAdd(item, weight)) {
                _weights[item] = weight;
            }
            else {
                _items.Add(item);
            }

            _isDirty = true;
        }

        /// <summary>
        /// 移除项
        /// </summary>
        /// <param name="item">要移除的项</param>
        public void Remove(T item) {
            if (_weights.Remove(item)) {
                _items.Remove(item);
                _isDirty = true;
            }
        }

        /// <summary>
        /// 清空所有项
        /// </summary>
        public void Clear() {
            _weights.Clear();
            _items.Clear();
            _cumulativeWeights.Clear();
            _totalWeight = 0;
            _isDirty = false;
        }

        /// <summary>
        /// 选择单个随机项
        /// </summary>
        /// <returns>根据权重随机选择的项</returns>
        public T SelectOne() {
            if (_weights.Count == 0)
                throw new InvalidOperationException("No items available for selection");

            RecalculateIfNeeded();

            double randomValue = _random.NextDouble() * _totalWeight;
            int selectedIndex = _cumulativeWeights.BinarySearch(randomValue);

            if (selectedIndex < 0)
                selectedIndex = ~selectedIndex;

            return _items[selectedIndex];
        }

        /// <summary>
        /// 选择多个不重复的随机项
        /// </summary>
        /// <param name="count">要选择的项数</param>
        /// <returns>根据权重随机选择的不重复项集合</returns>
        public IEnumerable<T> SelectMultiple(int count) {
            if (count <= 0)
                throw new ArgumentOutOfRangeException(nameof(count), "Count must be positive");

            if (count > _weights.Count)
                throw new ArgumentOutOfRangeException(nameof(count), "Count cannot exceed the number of available items");

            // 临时复制当前状态
            var tempItems = new List<T>(_items);
            var tempWeights = new List<double>(_items.Select(item => _weights[item]));

            for (int i = 0; i < count; i++) {
                // 计算当前总权重
                double currentTotalWeight = tempWeights.Sum();

                // 随机选择
                double randomValue = _random.NextDouble() * currentTotalWeight;
                double cumulativeWeight = 0;
                int selectedIndex = 0;

                for (int j = 0; j < tempWeights.Count; j++) {
                    cumulativeWeight += tempWeights[j];
                    if (randomValue <= cumulativeWeight) {
                        selectedIndex = j;
                        break;
                    }
                }

                // 返回选中的项
                yield return tempItems[selectedIndex];

                // 从临时集合中移除已选项
                tempItems.RemoveAt(selectedIndex);
                tempWeights.RemoveAt(selectedIndex);
            }
        }

        private void RecalculateIfNeeded() {
            if (!_isDirty) return;

            _cumulativeWeights.Clear();
            _totalWeight = 0;

            foreach (var item in _items) {
                _totalWeight += _weights[item];
                _cumulativeWeights.Add(_totalWeight);
            }

            _isDirty = false;
        }
    }

    /// <summary>
    /// 整数范围带权重随机选择器
    /// </summary>
    public class IntRangeWeightedRandomSelector : WeightedRandomSelector<int> {
        /// <summary>
        /// 初始化整数范围选择器
        /// </summary>
        /// <param name="minValue">最小值(包含)</param>
        /// <param name="maxValue">最大值(包含)</param>
        /// <param name="defaultWeight">默认权重</param>
        public IntRangeWeightedRandomSelector(int minValue, int maxValue, double defaultWeight = 1.0) {
            if (minValue > maxValue)
                throw new ArgumentException("minValue cannot be greater than maxValue");

            for (int i = minValue; i <= maxValue; i++) {
                AddOrUpdateWeight(i, defaultWeight);
            }
        }
    }
}