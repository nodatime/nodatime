using NUnit.Framework;
using NodaTime.Text;

namespace NodaTime.Test.Text
{
    /// <summary>
    /// Base class for tests of classes derived from TextCursor.
    /// </summary>
    public abstract class TextCursorTestBase
    {
        [Test]
        public void TestConstructor()
        {
            const string testString = "test";
            TextCursor cursor = MakeCursor(testString);
            ValidateContents(cursor, testString);
            ValidateBeginningOfString(cursor);
        }

        [Test]
        public void TestMove()
        {
            TextCursor cursor = MakeCursor("test");
            ValidateBeginningOfString(cursor);
            Assert.True(cursor.Move(0));
            ValidateCurrentCharacter(cursor, 0, 't');
            Assert.True(cursor.Move(1));
            ValidateCurrentCharacter(cursor, 1, 'e');
            Assert.True(cursor.Move(2));
            ValidateCurrentCharacter(cursor, 2, 's');
            Assert.True(cursor.Move(3));
            ValidateCurrentCharacter(cursor, 3, 't');
            Assert.False(cursor.Move(4));
            ValidateEndOfString(cursor);
        }

        [Test]
        public void TestMove_NextPrevious()
        {
            TextCursor cursor = MakeCursor("test");
            ValidateBeginningOfString(cursor);
            Assert.True(cursor.Move(2), "Move(2)");
            ValidateCurrentCharacter(cursor, 2, 's');
            Assert.True(cursor.MovePrevious(), "MovePrevious()");
            ValidateCurrentCharacter(cursor, 1, 'e');
            Assert.True(cursor.MoveNext(), "MoveNext()");
            ValidateCurrentCharacter(cursor, 2, 's');
        }

        [Test]
        public void TestMove_invalid()
        {
            TextCursor cursor = MakeCursor("test");
            ValidateBeginningOfString(cursor);
            Assert.False(cursor.Move(-1000));
            ValidateBeginningOfString(cursor);
            Assert.False(cursor.Move(1000));
            ValidateEndOfString(cursor);
            Assert.False(cursor.Move(-1000));
            ValidateBeginningOfString(cursor);
        }

        internal char GetNextCharacter(TextCursor cursor)
        {
            Assert.IsTrue(cursor.MoveNext());
            return cursor.Current;
        }

        internal static void ValidateBeginningOfString(TextCursor cursor)
        {

            ValidateCurrentCharacter(cursor, -1, TextCursor.Nul);
        }

        internal static void ValidateCurrentCharacter(TextCursor cursor, int expectedCurrentIndex, char expectedCurrentCharacter)
        {
            TestHelper.AssertCharEqual(expectedCurrentCharacter, cursor.Current);
            Assert.AreEqual(expectedCurrentIndex, cursor.Index);
        }

        internal static void ValidateEndOfString(TextCursor cursor)
        {
            ValidateCurrentCharacter(cursor, cursor.Length, TextCursor.Nul);
        }

        internal static void ValidateContents(TextCursor cursor, string value)
        {
            ValidateContents(cursor, value, -1);
        }

        internal static void ValidateContents(TextCursor cursor, string value, int length)
        {
            if (length < 0)
            {
                length = value.Length;
            }
            Assert.AreEqual(value, cursor.Value, "Cursor Value mismatch");
            Assert.AreEqual(length, cursor.Length, "Cursor Length mismatch");
        }

        internal abstract TextCursor MakeCursor(string value);
    }
}
