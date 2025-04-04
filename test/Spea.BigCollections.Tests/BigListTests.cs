namespace BigCollections.Tests
{
    public class BigListTests
    {
        [Fact]
        public void ShouldInitializeWithDefaultCapacity()
        {
            // Arrange & Act
            var bigList = new BigList<int>();

            // Assert
            Assert.Equal(0, bigList.Count);
            Assert.Equal(0, bigList.Capacity);
        }

        [Fact]
        public void ShouldInitializeWithSpecifiedCapacity()
        {
            // Arrange
            long capacity = 100;

            // Act
            var bigList = new BigList<int>(capacity);

            // Assert
            Assert.Equal(0, bigList.Count);
            Assert.Equal(capacity, bigList.Capacity);
        }

        [Fact]
        public void ShouldInitializeWithCollection()
        {
            // Arrange
            var collection = new List<int> { 1, 2, 3, 4 };

            // Act
            var bigList = new BigList<int>(collection);

            // Assert
            Assert.Equal(collection.Count, bigList.Count);
            for (int i = 0; i < collection.Count; i++)
            {
                Assert.Equal(collection[i], bigList[i]);
            }
        }

        [Fact]
        public void ShouldAddElements()
        {
            // Arrange
            var bigList = new BigList<int>();

            // Act
            bigList.Add(1);
            bigList.Add(2);

            // Assert
            Assert.Equal(2, bigList.Count);
            Assert.Equal(1, bigList[0]);
            Assert.Equal(2, bigList[1]);
        }

        [Fact]
        public void ShouldInsertElementAtIndex()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };

            // Act
            bigList.Insert(1, 42);

            // Assert
            Assert.Equal(4, bigList.Count);
            Assert.Equal(1, bigList[0]);
            Assert.Equal(42, bigList[1]);
            Assert.Equal(2, bigList[2]);
            Assert.Equal(3, bigList[3]);
        }

        [Fact]
        public void ShouldRemoveElementAtIndex()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };

            // Act
            bigList.RemoveAt(1);

            // Assert
            Assert.Equal(2, bigList.Count);
            Assert.Equal(1, bigList[0]);
            Assert.Equal(3, bigList[1]);
        }

        [Fact]
        public void ShouldClearAllElements()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };

            // Act
            bigList.Clear();

            // Assert
            Assert.Equal(0, bigList.Count);
        }

        [Fact]
        public void ShouldContainElement()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };

            // Act & Assert
            Assert.True(bigList.Contains(2));
            Assert.False(bigList.Contains(42));
        }

        [Fact]
        public void ShouldCopyToArray()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };
            var array = new int[3];

            // Act
            bigList.CopyTo(0, array, 3);

            // Assert
            Assert.Equal(bigList[0], array[0]);
            Assert.Equal(bigList[1], array[1]);
            Assert.Equal(bigList[2], array[2]);
        }

        [Fact]
        public void ShouldTrimExcess()
        {
            // Arrange
            var bigList = new BigList<int>(100);
            bigList.Add(1);
            bigList.Add(2);

            // Act
            bigList.TrimExcess();

            // Assert
            Assert.Equal(2, bigList.Count);
            Assert.Equal(2, bigList.Capacity);
        }

        [Fact]
        public void ShouldTrimToSize()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3, 4, 5 };

            // Act
            bigList.TrimToSize(3);

            // Assert
            Assert.Equal(3, bigList.Count);
            Assert.Equal(3, bigList.Capacity);
        }

        [Fact]
        public void ShouldEnumerateElements()
        {
            // Arrange
            var bigList = new BigList<int> { 1, 2, 3 };
            var enumeratedList = new List<int>();

            // Act
            foreach (var item in bigList)
            {
                enumeratedList.Add(item);
            }

            // Assert
            Assert.Equal(bigList.Count, enumeratedList.Count);
            for (int i = 0; i < bigList.Count; i++)
            {
                Assert.Equal(bigList[i], enumeratedList[i]);
            }
        }
    }
}
