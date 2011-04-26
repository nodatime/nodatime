#region Copyright and license information
// Copyright 2001-2009 Stephen Colebourne
// Copyright 2009-2011 Jon Skeet
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region usings
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
#endregion

namespace NodaTime.Format.Builder
{
    internal sealed class FormatterBuilder<T, TInfo>
        where TInfo : ParseInfo
    {
        private readonly IList<IFormatNode<TInfo>> nodes = new List<IFormatNode<TInfo>>();

        internal void Add(IFormatNode<TInfo> node)
        {
            nodes.Add(node);
        }

        public void AddString(string text)
        {
            Add(new StringNode(text));
        }

        public void AddDateSeparator()
        {
            Add(new DateSeparaterNode());
        }

        public void AddTimeSeparator()
        {
            Add(new TimeSeparaterNode());
        }

        public void AddDecimalSeparator()
        {
            Add(new DecimalSeparaterNode());
        }

        public void AddSign(bool required, Func<TInfo, ISignedValue> getValue)
        {
            Add(new SignNode(required, getValue));
        }

        public void AddLeftPad(int width, Func<TInfo, int> getValue)
        {
            Add(new LeftPadNode(width, getValue));
        }

        public void AddRightPad(int width, int scale, Func<TInfo, int> getValue)
        {
            Add(new RightPadNode(width, scale, getValue));
        }

        public void AddRightPadTruncate(int width, int scale, Func<TInfo, int> getValue)
        {
            Add(new RightPadTruncateNode(width, scale, getValue));
        }

        public INodaFormatter<T> Build()
        {
            return null;
        }

        private class PatternFormatter : AbstractNodaFormatter<T>
        {
            private readonly IFormatNode<TInfo>[] nodes;

            public PatternFormatter(ICollection<IFormatNode<TInfo>> patternList, IFormatProvider formatProvider) : base(formatProvider)
            {
                nodes = new IFormatNode<TInfo>[patternList.Count];
                patternList.CopyTo(nodes, 0);
            }

            public override string Format(T value, IFormatProvider formatProvider)
            {

            }

            public override INodaFormatter<T> WithFormatProvider(IFormatProvider formatProvider)
            {
                throw new NotImplementedException();
            }
        }

        #region Nested type: DateSeparaterNode
        private sealed class DateSeparaterNode : IFormatNode<TInfo>
        {
            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                builder.Append(info.FormatInfo.DateSeparator);
            }
            #endregion
        }
        #endregion

        #region Nested type: DecimalSeparaterNode
        private sealed class DecimalSeparaterNode : IFormatNode<TInfo>
        {
            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                builder.Append(info.FormatInfo.DecimalSeparator);
            }
            #endregion
        }
        #endregion

        #region Nested type: LeftPadNode
        private sealed class LeftPadNode : IFormatNode<TInfo>
        {
            private readonly Func<TInfo, int> getValue;
            private readonly int width;

            public LeftPadNode(int width, Func<TInfo, int> getValue)
            {
                this.width = width;
                this.getValue = getValue;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                int value = getValue(info);
                FormatHelper.LeftPad(value, width, builder);
            }
            #endregion
        }
        #endregion

        #region Nested type: RightPadNode
        private sealed class RightPadNode : IFormatNode<TInfo>
        {
            private readonly Func<TInfo, int> getValue;
            private readonly int scale;
            private readonly int width;

            public RightPadNode(int width, int scale, Func<TInfo, int> getValue)
            {
                this.width = width;
                this.scale = scale;
                this.getValue = getValue;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                int value = getValue(info);
                FormatHelper.RightPad(value, width, scale, builder);
            }
            #endregion
        }
        #endregion

        #region Nested type: RightPadTruncateNode
        private sealed class RightPadTruncateNode : IFormatNode<TInfo>
        {
            private readonly Func<TInfo, int> getValue;
            private readonly int scale;
            private readonly int width;

            public RightPadTruncateNode(int width, int scale, Func<TInfo, int> getValue)
            {
                this.width = width;
                this.scale = scale;
                this.getValue = getValue;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                int value = getValue(info);
                var nfi = NumberFormatInfo.GetInstance(formatProvider);
                FormatHelper.RightPadTruncate(value, width, scale, nfi.NumberDecimalSeparator, builder);
            }
            #endregion
        }
        #endregion

        #region Nested type: SignNode
        private sealed class SignNode : IFormatNode<TInfo>
        {
            private readonly Func<TInfo, ISignedValue> getValue;
            private readonly bool required;

            public SignNode(bool required, Func<TInfo, ISignedValue> getValue)
            {
                this.required = required;
                this.getValue = getValue;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                var value = getValue(info);
                FormatHelper.FormatSign(value, required, builder);
            }
            #endregion
        }
        #endregion

        #region Nested type: StringNode
        private sealed class StringNode : IFormatNode<TInfo>
        {
            private readonly string text;

            public StringNode(string text)
            {
                this.text = text;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                builder.Append(text);
            }
            #endregion
        }
        #endregion

        #region Nested type: TimeSeparaterNode
        private sealed class TimeSeparaterNode : IFormatNode<TInfo>
        {
            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder, IFormatProvider formatProvider)
            {
                builder.Append(info.FormatInfo.TimeSeparator);
            }
            #endregion
        }
        #endregion
    }
}
