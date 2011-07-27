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
    internal sealed class FormatterBuilder<T, TInfo> where TInfo : ParseInfo
    {
        #region Delegates

        /// <summary>
        /// Equivalent to Func[T], but without requiring .NET 3.5. If we ever require .NET 3.5,
        /// we can remove this.
        /// </summary>
        internal delegate TResult NodaFunc<TArg, TResult>(TArg input);

        public delegate TInfo MakeFormattingParseInfo(T value, IFormatProvider formatProvider);

        public delegate TInfo MakeParsingParseInfo(IFormatProvider formatProvider, bool throwImmediate, DateTimeParseStyles styles);
        #endregion

        private readonly IList<IFormatterBuilderNode<TInfo>> nodes = new List<IFormatterBuilderNode<TInfo>>();

        internal FormatterBuilder(string format)
        {
            FormatPattern = format;
        }

        internal string FormatPattern { get; private set; }

        public override string ToString()
        {
            return "FormatterBuilder for \"" + FormatPattern + "\"";
        }

        internal void Add(IFormatterBuilderNode<TInfo> node)
        {
            nodes.Add(node);
        }

        public void AddString(string text)
        {
            Add(new StringBuilderNode(text));
        }

        public void AddDateSeparator()
        {
            Add(new DateSeparaterBuilderNode());
        }

        public void AddTimeSeparator()
        {
            Add(new TimeSeparaterBuilderNode());
        }

        public void AddDecimalSeparator()
        {
            Add(new DecimalSeparaterBuilderNode());
        }

        public void AddSign(bool required, NodaFunc<TInfo, ISignedValue> getValue)
        {
            Add(new SignBuilderNode(required, getValue));
        }

        public void AddLeftPad(int width, NodaFunc<TInfo, int> getValue)
        {
            Add(new LeftPadBuilderNode(width, getValue));
        }

        public void AddRightPad(int width, int scale, NodaFunc<TInfo, int> getValue)
        {
            Add(new RightPadeBuilderNode(width, scale, getValue));
        }

        public void AddRightPadTruncate(int width, int scale, NodaFunc<TInfo, int> getValue)
        {
            Add(new RightPadTruncateBuilderNode(width, scale, getValue));
        }

        public INodaFormatter<T> Build(IFormatProvider formatProvider, MakeFormattingParseInfo makeInfo)
        {
            return new PatternFormatter(FormatPattern, formatProvider, nodes, makeInfo);
        }

        #region Nested type: DateSeparaterBuilderNode
        private sealed class DateSeparaterBuilderNode : IFormatterBuilderNode<TInfo>
        {
            #region IFormatterBuilderNode<TInfo> Members
            public IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider)
            {
                DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(formatProvider);
                return new StringNode(info.DateSeparator);
            }
            #endregion
        }
        #endregion

        #region Nested type: DecimalSeparaterBuilderNode
        private sealed class DecimalSeparaterBuilderNode : IFormatterBuilderNode<TInfo>
        {
            #region IFormatterBuilderNode<TInfo> Members
            public IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider)
            {
                NumberFormatInfo info = NumberFormatInfo.GetInstance(formatProvider);
                return new StringNode(info.NumberDecimalSeparator);
            }
            #endregion
        }
        #endregion

        #region Nested type: LeftPadBuilderNode
        private sealed class LeftPadBuilderNode : AbstractFormatterBuilderNode<TInfo>
        {
            public LeftPadBuilderNode(int width, NodaFunc<TInfo, int> getValue) : base(new LeftPadNode(width, getValue))
            {
            }

            #region Nested type: LeftPadNode
            private sealed class LeftPadNode : IFormatNode<TInfo>
            {
                private readonly NodaFunc<TInfo, int> getValue;
                private readonly int width;

                public LeftPadNode(int width, NodaFunc<TInfo, int> getValue)
                {
                    this.width = width;
                    this.getValue = getValue;
                }

                #region IFormatNode<TInfo> Members
                public void Append(TInfo info, StringBuilder builder)
                {
                    int value = getValue(info);
                    FormatHelper.LeftPad(value, width, builder);
                }
                #endregion

                public override string ToString()
                {
                    return "Format left pad: width=" + width;
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: PatternFormatter
        private sealed class PatternFormatter : AbstractNodaFormatter<T>
        {
            private readonly string formatPattern;
            private readonly MakeFormattingParseInfo makeInfo;
            private readonly IList<IFormatNode<TInfo>> nodes;
            private readonly ICollection<IFormatterBuilderNode<TInfo>> patternList;

            public PatternFormatter(string format, IFormatProvider formatProvider, ICollection<IFormatterBuilderNode<TInfo>> patternList,
                                    MakeFormattingParseInfo makeInfo) : base(formatProvider)
            {
                formatPattern = format;
                this.patternList = patternList;
                nodes = new List<IFormatNode<TInfo>>(patternList.Count);
                this.makeInfo = makeInfo;
                foreach (var node in patternList)
                {
                    var formatNode = node.MakeNode(formatProvider);
                    int count = nodes.Count;
                    if (count > 0 && formatNode is StringNode && nodes[count - 1] is StringNode)
                    {
                        var snode = (StringNode)nodes[count - 1];
                        snode.Text += ((StringNode)formatNode).Text;
                    }
                    else
                    {
                        nodes.Add(formatNode);
                    }
                }
            }

            /// <summary>
            /// Formats the specified value using the <see cref="IFormatProvider"/> given when the formatter
            /// was constructed. This does NOT use the current thread <see cref="IFormatProvider"/>.
            /// </summary>
            /// <param name="value">The value to format.</param>
            /// <returns>The value formatted as a string.</returns>
            public override string Format(T value)
            {
                TInfo info = makeInfo(value, FormatProvider);
                var builder = new StringBuilder();
                foreach (var formatNode in nodes)
                {
                    formatNode.Append(info, builder);
                }
                return builder.ToString();
            }

            /// <summary>
            /// Returns a new copy of this formatter that uses the given <see cref="IFormatProvider"/> for
            /// formatting instead of the one that this formatter uses.
            /// </summary>
            /// <param name="formatProvider">The format provider to use.</param>
            /// <returns>A new copy of this formatter using the given <see cref="IFormatProvider"/>.</returns>
            public override INodaFormatter<T> WithFormatProvider(IFormatProvider formatProvider)
            {
                return new PatternFormatter(formatPattern, formatProvider, patternList, makeInfo);
            }

            /// <summary>
            /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
            /// </returns>
            /// <filterpriority>2</filterpriority>
            public override string ToString()
            {
                return "Formatter for \"" + formatPattern + "\"";
            }
        }
        #endregion

        #region Nested type: RightPadTruncateBuilderNode
        private sealed class RightPadTruncateBuilderNode : AbstractFormatterBuilderNode<TInfo>
        {
            public RightPadTruncateBuilderNode(int width, int scale, NodaFunc<TInfo, int> getValue) : base(new RightPadTruncateNode(width, scale, getValue))
            {
            }

            #region Nested type: RightPadTruncateNode
            private sealed class RightPadTruncateNode : IFormatNode<TInfo>
            {
                private readonly NodaFunc<TInfo, int> getValue;
                private readonly int scale;
                private readonly int width;

                public RightPadTruncateNode(int width, int scale, NodaFunc<TInfo, int> getValue)
                {
                    this.width = width;
                    this.scale = scale;
                    this.getValue = getValue;
                }

                #region IFormatNode<TInfo> Members
                public void Append(TInfo info, StringBuilder builder)
                {
                    int value = getValue(info);
                    NumberFormatInfo nfi = NumberFormatInfo.GetInstance(info.FormatProvider);
                    FormatHelper.RightPadTruncate(value, width, scale, nfi.NumberDecimalSeparator, builder);
                }
                #endregion

                public override string ToString()
                {
                    return "Format right pad truncate: width=" + 3 + " scale=" + scale;
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: RightPadeBuilderNode
        private sealed class RightPadeBuilderNode : AbstractFormatterBuilderNode<TInfo>
        {
            public RightPadeBuilderNode(int width, int scale, NodaFunc<TInfo, int> getValue) : base(new RightPadNode(width, scale, getValue))
            {
            }

            #region Nested type: RightPadNode
            private sealed class RightPadNode : IFormatNode<TInfo>
            {
                private readonly NodaFunc<TInfo, int> getValue;
                private readonly int scale;
                private readonly int width;

                public RightPadNode(int width, int scale, NodaFunc<TInfo, int> getValue)
                {
                    this.width = width;
                    this.scale = scale;
                    this.getValue = getValue;
                }

                #region IFormatNode<TInfo> Members
                public void Append(TInfo info, StringBuilder builder)
                {
                    int value = getValue(info);
                    FormatHelper.RightPad(value, width, scale, builder);
                }
                #endregion

                public override string ToString()
                {
                    return "Format right pad: width=" + 3 + " scale=" + scale;
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: SignBuilderNode
        private sealed class SignBuilderNode : IFormatterBuilderNode<TInfo>
        {
            private readonly NodaFunc<TInfo, ISignedValue> getValue;
            private readonly bool required;

            public SignBuilderNode(bool required, NodaFunc<TInfo, ISignedValue> getValue)
            {
                this.required = required;
                this.getValue = getValue;
            }

            #region IFormatterBuilderNode<TInfo> Members
            public IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider)
            {
                var info = NumberFormatInfo.GetInstance(formatProvider);
                if (required)
                {
                    return new SignRequiredNode(info.PositiveSign, info.NegativeSign, getValue);
                }
                return new SignOptionalNode(info.NegativeSign, getValue);
            }
            #endregion

            #region Nested type: SignOptionalNode
            private sealed class SignOptionalNode : IFormatNode<TInfo>
            {
                private readonly NodaFunc<TInfo, ISignedValue> getValue;
                private readonly string negative;

                public SignOptionalNode(string negative, NodaFunc<TInfo, ISignedValue> getValue)
                {
                    this.getValue = getValue;
                    this.negative = negative;
                }

                #region IFormatNode<TInfo> Members
                public void Append(TInfo info, StringBuilder builder)
                {
                    ISignedValue value = getValue(info);
                    if (value.IsNegative)
                    {
                        builder.Append(negative);
                    }
                }
                #endregion

                public override string ToString()
                {
                    return "Format sign: optional";
                }
            }
            #endregion

            #region Nested type: SignRequiredNode
            private sealed class SignRequiredNode : IFormatNode<TInfo>
            {
                private readonly NodaFunc<TInfo, ISignedValue> getValue;
                private readonly string negative;
                private readonly string positive;

                public SignRequiredNode(string positive, string negative, NodaFunc<TInfo, ISignedValue> getValue)
                {
                    this.getValue = getValue;
                    this.positive = positive;
                    this.negative = negative;
                }

                #region IFormatNode<TInfo> Members
                public void Append(TInfo info, StringBuilder builder)
                {
                    ISignedValue value = getValue(info);
                    builder.Append(value.IsNegative ? negative : positive);
                }
                #endregion

                public override string ToString()
                {
                    return "Format sign: required";
                }
            }
            #endregion
        }
        #endregion

        #region Nested type: StringBuilderNode
        private sealed class StringBuilderNode : AbstractFormatterBuilderNode<TInfo>
        {
            public StringBuilderNode(string text) : base(new StringNode(text))
            {
            }
        }
        #endregion

        #region Nested type: StringNode
        private sealed class StringNode : IFormatNode<TInfo>
        {
            internal string Text { get; set; }

            public StringNode(string text)
            {
                Text = text;
            }

            #region IFormatNode<TInfo> Members
            public void Append(TInfo info, StringBuilder builder)
            {
                builder.Append(Text);
            }
            #endregion

            public override string ToString()
            {
                return "Format string: [" + Text + "]";
            }
        }
        #endregion

        #region Nested type: TimeSeparaterBuilderNode
        private sealed class TimeSeparaterBuilderNode : IFormatterBuilderNode<TInfo>
        {
            #region IFormatterBuilderNode<TInfo> Members
            public IFormatNode<TInfo> MakeNode(IFormatProvider formatProvider)
            {
                DateTimeFormatInfo info = DateTimeFormatInfo.GetInstance(formatProvider);
                return new StringNode(info.TimeSeparator);
            }
            #endregion
        }
        #endregion
    }
}