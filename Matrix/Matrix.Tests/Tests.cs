using System;
using System.IO;
using NUnit.Framework;

namespace Matrix.Tests
{
    public class Tests
    {
        [Test]
        public void MultiplicationTest()
        {
            var matrix1 = new int[,]
            {
                {1, 1},
                {1, 1}
            };
            var matrix2 = new int[,]
            {
                {4, 4},
                {3, 4}
            };
            var rightResult1 = new int[,]
            {
                {7, 8},
                {7, 8}
            };
            var parallelResult1 = matrix1.ParallelMultiply(matrix2);
            var standardResult1 = matrix1.StandardMultiply(matrix2);
            Assert.AreEqual(rightResult1, parallelResult1);
            Assert.AreEqual(rightResult1, standardResult1);

            var matrix3 = new int[,]
            {
                {2, 6},
                {2, 9},
                {8, 7}
            };
            var matrix4 = new int[,]
            {
                {1, 2, 3},
                {6, 5, 4}
            };
            var rightResult2 = new int[,]
            {
                {38, 34, 30},
                {56, 49, 42},
                {50, 51, 52}
            };
            var parallelResult2 = matrix3.ParallelMultiply(matrix4);
            var standardResult2 = matrix3.StandardMultiply(matrix4);
            Assert.AreEqual(rightResult2, parallelResult2);
            Assert.AreEqual(rightResult2, standardResult2);

            var matrix5 = new int[,] {{0}};
            var matrix6 = new int[,] {{12}};
            var rightResult3 = new int[,] {{0}};
            var parallelResult3 = matrix5.ParallelMultiply(matrix6);
            var standardResult3 =matrix5.StandardMultiply(matrix6);
            Assert.AreEqual(rightResult3, parallelResult3);
            Assert.AreEqual(rightResult3, standardResult3);
        }

        [Test]
        public void RandomizedMultiplicationTest()
        {
            var rand = new Random();
            for (var i = 0; i < 1000; i++)
            {
                var rows1 = rand.Next(1, 64);
                var cols1AndRows1 = rand.Next(1, 64);
                var cols2 = rand.Next(1, 64);
                var matrixA = Generator.Generate(rows1, cols1AndRows1);
                var matrixB = Generator.Generate(cols1AndRows1, cols2);
                var parallelResult1 = matrixA.StandardMultiply(matrixB);
                var standardResult1 = matrixA.ParallelMultiply(matrixB);
                Assert.AreEqual(parallelResult1, standardResult1);
            }
        }

        [Test]
        public void InvalidSizesMultiplicationExceptionTest()
        {
            var matrixA = new int[2, 8];
            var matrixB = new int[2, 8];
            Assert.Throws<InvalidSizesMultiplicationException>(() => matrixA.StandardMultiply(matrixB));
            Assert.Throws<InvalidSizesMultiplicationException>(() => matrixA.ParallelMultiply(matrixB));
        }
        
        [Test]
        public void ArgumentOutOfRangeExceptionTest()
        {
            Assert.Throws<InvalidSizeMatrixException>(() => Generator.Generate(228, -1337));
            Assert.Throws<InvalidSizeMatrixException>(() => Generator.Generate(-14, 13));
        }

        [Test]
        public void WriteAndReadShouldWorkCorrectly()
        {
            var (rows, cols) = (50, 50);
            var path = $"{Path.GetTempPath()}/test.txt";
            var generatedMatrix = Generator.Generate(rows, cols);
            PrintMatrix.Print(generatedMatrix, path);
            Assert.AreEqual(generatedMatrix, ReadMatrix.Read(path));
        }
    }
}