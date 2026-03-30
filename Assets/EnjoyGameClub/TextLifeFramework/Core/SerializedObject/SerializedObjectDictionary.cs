/*
 * This Source Code Form is subject to the terms of the Mozilla Public License, v. 2.0.
 * If a copy of the MPL was not distributed with this file, You can obtain one at https://mozilla.org/MPL/2.0/.
 *
 * Copyright (c) Ruoy
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnjoyGameClub.TextLifeFramework.Core.SerializedObject
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {
        [SerializeReference] public List<TKey> _keysList = new();

        [SerializeReference] public List<TValue> _valuesList = new();

        public TValue this[TKey key]
        {
            get
            {
                var index = _keysList.IndexOf(key);
                if (index >= 0)
                    return _valuesList[index];
                throw new KeyNotFoundException();
            }
            set
            {
                var index = _keysList.IndexOf(key);
                if (index >= 0)
                {
                    _valuesList[index] = value;
                }
                else
                {
                    _keysList.Add(key);
                    _valuesList.Add(value);
                }
            }
        }

        public ICollection<TKey> Keys => _keysList;
        public ICollection<TValue> Values => _valuesList;
        public int Count => _keysList.Count;
        public bool IsReadOnly => false;

        public void Add(TKey key, TValue value)
        {
            if (_keysList.Contains(key))
                throw new ArgumentException("An element with the same key already exists.");
            _keysList.Add(key);
            _valuesList.Add(value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        public void Clear()
        {
            _keysList.Clear();
            _valuesList.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            var index = _keysList.IndexOf(item.Key);
            return index >= 0 && EqualityComparer<TValue>.Default.Equals(_valuesList[index], item.Value);
        }

        public bool ContainsKey(TKey key)
        {
            return _keysList.Contains(key);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            for (var i = 0; i < _keysList.Count; i++)
                array[arrayIndex + i] = new KeyValuePair<TKey, TValue>(_keysList[i], _valuesList[i]);
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            for (var i = 0; i < _keysList.Count; i++)
                yield return new KeyValuePair<TKey, TValue>(_keysList[i], _valuesList[i]);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Remove(TKey key)
        {
            var index = _keysList.IndexOf(key);
            if (index >= 0)
            {
                _keysList.RemoveAt(index);
                _valuesList.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            var index = _keysList.IndexOf(item.Key);
            if (index >= 0 && EqualityComparer<TValue>.Default.Equals(_valuesList[index], item.Value))
            {
                _keysList.RemoveAt(index);
                _valuesList.RemoveAt(index);
                return true;
            }

            return false;
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = _keysList.IndexOf(key);
            if (index >= 0)
            {
                value = _valuesList[index];
                return true;
            }

            value = default;
            return false;
        }
    }
}