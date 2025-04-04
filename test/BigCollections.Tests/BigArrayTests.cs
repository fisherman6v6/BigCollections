namespace BigCollections.Tests
{
    public class BigArrayTests
    {
        [Fact]
        public void ShouldInitializeWithCorrectSize()
        {
            int size = 1000;
            var bigArray = new BigArray<int>(size);
            Assert.Equal(size, bigArray.Length);
        }

        [Fact]
        public void ShouldSetAndGetValuesCorrectly()
        {
            int size = 1000;
            var bigArray = new BigArray<int>(size);
            int index = 500;
            int value = 42;

            bigArray[index] = value;
            Assert.Equal(value, bigArray[index]);
        }

        [Fact]
        public void ShouldThrowExceptionForInvalidIndex()
        {
            int size = 1000;
            var bigArray = new BigArray<int>(size);

            Assert.Throws<IndexOutOfRangeException>(() => bigArray[-1] = 42);
            Assert.Throws<IndexOutOfRangeException>(() => bigArray[size] = 42);
        }

        [Fact]
        public void ShouldResizeCorrectly()
        {
            int initialSize = 1000;
            var bigArray = new BigArray<int>(initialSize);
            int newSize = 2000;

            bigArray.Resize(newSize);
            Assert.Equal(newSize, bigArray.Length);
        }

        [Fact]
        public void ShouldClearArray()
        {
            int size = 1000;
            var bigArray = new BigArray<int>(size);
            bigArray[500] = 42;

            bigArray.Clear();
            Assert.Equal(0, bigArray[500]);
        }

        [Fact]
        public void ShouldCompareArraysCorrectly()
        {
            var bigArray1 = new BigArray<int>(1000);
            var bigArray2 = new BigArray<int>(1000);

            Assert.Equal(bigArray1, bigArray2);

            bigArray1[500] = 42;
            Assert.NotEqual(bigArray1, bigArray2);
        }
    }
}
