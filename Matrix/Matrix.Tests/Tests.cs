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
            var matrix1 = new Matrix( new [,]
            {
                {1, 1},
                {1, 1}
            });
            var matrix2 = new Matrix( new [,]
            {
                {4, 4},
                {3, 4}
            });
            var rightResult1 = new Matrix( new [,]
            {
                {7, 8},
                {7, 8}
            });
            var parallelResult1 = matrix1.Multiply(matrix2, true);
            var standardResult1 = matrix1.Multiply(matrix2);
            Assert.AreEqual(rightResult1, parallelResult1);
            Assert.AreEqual(rightResult1, standardResult1);

            var matrix3 = new Matrix( new [,]
            {
                {2, 6},
                {2, 9},
                {8, 7}
            });
            var matrix4 = new Matrix( new [,]
            {
                {1, 2, 3},
                {6, 5, 4}
            });
            var rightResult2 = new Matrix( new [,]
            {
                {38, 34, 30},
                {56, 49, 42},
                {50, 51, 52}
            });
            var parallelResult2 = matrix3.Multiply(matrix4, true);
            var standardResult2 = matrix3.Multiply(matrix4);
            Assert.AreEqual(rightResult2, parallelResult2);
            Assert.AreEqual(rightResult2, standardResult2);

            var matrix5 = new Matrix(new [,] {{0}});
            var matrix6 = new Matrix(new [,] {{12}});
            var rightResult3 = new Matrix(new [,] {{0}});
            var parallelResult3 = matrix5.Multiply(matrix6, true);
            var standardResult3 =matrix5.Multiply(matrix6);
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
                var matrixA = Matrix.Generate(rows1, cols1AndRows1);
                var matrixB = Matrix.Generate(cols1AndRows1, cols2);
                var parallelResult1 = matrixA.Multiply(matrixB, true);
                var standardResult1 = matrixA.Multiply(matrixB);
                Assert.AreEqual(parallelResult1, standardResult1);
            }
        }

        [Test]
        public void InvalidSizesMultiplicationExceptionTest()
        {
            var matrixA = new Matrix(2, 8);
            var matrixB = new Matrix(2, 8);
            Assert.Throws<InvalidSizesMultiplicationException>(() => matrixA.Multiply(matrixB));
            Assert.Throws<InvalidSizesMultiplicationException>(() => matrixA.Multiply(matrixB, true));
        }

        [Test]
        public void WriteAndReadShouldWorkCorrectly()
        {
            var (rows, cols) = (50, 50);
            var path = $"{Path.GetTempPath()}/test.txt";
            var generatedMatrix = Matrix.Generate(rows, cols);
            Matrix.Print(generatedMatrix, path); 
            Assert.AreEqual(generatedMatrix, Matrix.Read(path));
        }
    }
}