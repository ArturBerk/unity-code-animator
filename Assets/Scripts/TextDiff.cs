using System;
using System.Collections.Generic;
using System.Text;

namespace CodeAnimator
{
  internal sealed class DiagonalVector
  {
    private readonly int[] data;
    private readonly int max;

    public DiagonalVector(int n, int m)
    {
      int num = n - m;
      this.max = n + m + Math.Abs(num);
      this.data = new int[2 * this.max + 1];
    }

    public int this[int userIndex]
    {
      get
      {
        return this.data[this.GetActualIndex(userIndex)];
      }
      set
      {
        this.data[this.GetActualIndex(userIndex)] = value;
      }
    }

    private int GetActualIndex(int userIndex)
    {
      return userIndex + this.max;
    }
  }
  
  internal sealed class SubArray<T> where T : IComparable<T>
  {
    private readonly IList<T> data;
    private readonly int length;
    private readonly int offset;

    public SubArray(IList<T> data)
    {
      this.data = data;
      this.offset = 0;
      this.length = this.data.Count;
    }

    public SubArray(SubArray<T> data, int offset, int length)
    {
      this.data = data.data;
      this.offset = data.offset + offset - 1;
      this.length = length;
    }

    public int Length
    {
      get
      {
        return this.length;
      }
    }

    public int Offset
    {
      get
      {
        return this.offset;
      }
    }

    public T this[int index]
    {
      get
      {
        return this.data[this.offset + index - 1];
      }
    }

    public override string ToString()
    {
      StringBuilder stringBuilder = new StringBuilder(3 * this.length);
      for (int index = 0; index < this.length; ++index)
        stringBuilder.AppendFormat("{0} ", (object) this.data[this.offset + index]);
      return stringBuilder.ToString();
    }
  }
  
    public sealed class MyersDiff<T> where T : IComparable<T>
  {
    private readonly IList<T> listA;
    private readonly IList<T> listB;
    private readonly bool supportChangeEditType;
    private readonly DiagonalVector vectorForward;
    private readonly DiagonalVector vectorReverse;

    public MyersDiff(IList<T> listA, IList<T> listB, bool supportChangeEditType)
    {
      this.listA = listA;
      this.listB = listB;
      this.supportChangeEditType = supportChangeEditType;
      int count1 = listA.Count;
      int count2 = listB.Count;
      this.vectorForward = new DiagonalVector(count1, count2);
      this.vectorReverse = new DiagonalVector(count1, count2);
    }

    public List<TextChange> Execute()
    {
      List<MyersDiff<T>.Point> matchPoints = new List<MyersDiff<T>.Point>();
      SubArray<T> subArrayA = new SubArray<T>(this.listA);
      SubArray<T> subArrayB = new SubArray<T>(this.listB);
      this.GetMatchPoints(subArrayA, subArrayB, matchPoints);
      return this.ConvertMatchPointsToEditScript(subArrayA.Length, subArrayB.Length, matchPoints);
    }

    public IList<T> GetLongestCommonSubsequence()
    {
      List<T> output = new List<T>();
      this.GetLcs(new SubArray<T>(this.listA), new SubArray<T>(this.listB), output);
      return (IList<T>) output;
    }

    public int GetLongestCommonSubsequenceLength()
    {
      return (this.listA.Count + this.listB.Count - this.GetShortestEditScriptLength()) / 2;
    }

    public int GetReverseShortestEditScriptLength()
    {
      SubArray<T> subArrayA = new SubArray<T>(this.listA);
      SubArray<T> subArrayB = new SubArray<T>(this.listB);
      if (this.SetupFictitiousPoints(subArrayA, subArrayB))
      {
        int count1 = this.listA.Count;
        int count2 = this.listB.Count;
        int delta = count1 - count2;
        for (int d = 0; d <= count1 + count2; ++d)
        {
          for (int k = -d; k <= d; k += 2)
          {
            int reverseDpaths = this.GetReverseDPaths(subArrayA, subArrayB, d, k, delta);
            int num = reverseDpaths - (k + delta);
            if (reverseDpaths <= 0 && num <= 0)
              return d;
          }
        }
        return -1;
      }
      int count = this.listA.Count;
      return count != 0 ? count : this.listB.Count;
    }

    public int GetShortestEditScriptLength()
    {
      SubArray<T> subArrayA = new SubArray<T>(this.listA);
      SubArray<T> subArrayB = new SubArray<T>(this.listB);
      if (this.SetupFictitiousPoints(subArrayA, subArrayB))
      {
        int count1 = this.listA.Count;
        int count2 = this.listB.Count;
        for (int d = 0; d <= count1 + count2; ++d)
        {
          for (int k = -d; k <= d; k += 2)
          {
            int forwardDpaths = this.GetForwardDPaths(subArrayA, subArrayB, d, k);
            int num = forwardDpaths - k;
            if (forwardDpaths >= count1 && num >= count2)
              return d;
          }
        }
        return -1;
      }
      int count = this.listA.Count;
      return count != 0 ? count : this.listB.Count;
    }

    public double GetSimilarity()
    {
      return MyersDiff<T>.GetSimilarity(this.listA.Count, this.listB.Count, this.GetLongestCommonSubsequenceLength());
    }

    private static double GetSimilarity(int lengthA, int lengthB, int lengthLcs)
    {
      return 2.0 * (double) lengthLcs / (double) (lengthA + lengthB);
    }

    private List<TextChange> ConvertMatchPointsToEditScript(
      int n,
      int m,
      List<MyersDiff<T>.Point> matchPoints)
    {
      var result = new List<TextChange>();
      //EditScript editScript = new EditScript(MyersDiff<T>.GetSimilarity(n, m, matchPoints.Count));
      int num1 = 1;
      int num2 = 1;
      matchPoints.Add(new MyersDiff<T>.Point(n + 1, m + 1));
      foreach (MyersDiff<T>.Point matchPoint in matchPoints)
      {
        int x = matchPoint.X;
        int y = matchPoint.Y;
        if (this.supportChangeEditType && num1 < x && num2 < y)
        {
          int length = Math.Min(x - num1, y - num2);
          result.Add(new TextChange(ChangeType.Change, num1 - 1, num2 - 1, length));
          num1 += length;
          num2 += length;
        }
        if (num1 < x)
          result.Add(new TextChange(ChangeType.Delete, num1 - 1, num2 - 1, x - num1));
        if (num2 < y)
          result.Add(new TextChange(ChangeType.Insert, num1 - 1, num2 - 1, y - num2));
        num1 = x + 1;
        num2 = y + 1;
      }
      return result;
    }

    private int FindMiddleSnake(
      SubArray<T> subArrayA,
      SubArray<T> subArrayB,
      out int pathStartX,
      out int pathEndX,
      out int pathK)
    {
      this.SetupFictitiousPoints(subArrayA, subArrayB);
      pathStartX = -1;
      pathEndX = -1;
      pathK = 0;
      int delta = subArrayA.Length - subArrayB.Length;
      int num = (int) Math.Ceiling((double) (subArrayA.Length + subArrayB.Length) / 2.0);
      for (int d = 0; d <= num; ++d)
      {
        for (int k = -d; k <= d; k += 2)
        {
          this.GetForwardDPaths(subArrayA, subArrayB, d, k);
          if (delta % 2 != 0 && k >= delta - (d - 1) && (k <= delta + (d - 1) && this.vectorForward[k] >= this.vectorReverse[k]))
          {
            pathK = k;
            pathEndX = this.vectorForward[k];
            pathStartX = pathEndX;
            for (int index = pathStartX - pathK; pathStartX > 0 && index > 0 && subArrayA[pathStartX].CompareTo(subArrayB[index]) == 0; --index)
              --pathStartX;
            return 2 * d - 1;
          }
        }
        for (int k = -d; k <= d; k += 2)
        {
          this.GetReverseDPaths(subArrayA, subArrayB, d, k, delta);
          if (delta % 2 == 0 && k + delta >= -d && (k + delta <= d && this.vectorReverse[k + delta] <= this.vectorForward[k + delta]))
          {
            pathK = k + delta;
            pathStartX = this.vectorReverse[pathK];
            pathEndX = pathStartX;
            for (int index = pathEndX - pathK; pathEndX < subArrayA.Length && index < subArrayB.Length && subArrayA[pathEndX + 1].CompareTo(subArrayB[index + 1]) == 0; ++index)
              ++pathEndX;
            return 2 * d;
          }
        }
      }
      return -1;
    }

    private int GetForwardDPaths(SubArray<T> subArrayA, SubArray<T> subArrayB, int d, int k)
    {
      DiagonalVector vectorForward = this.vectorForward;
      int num = k == -d || k != d && vectorForward[k - 1] < vectorForward[k + 1] ? vectorForward[k + 1] : vectorForward[k - 1] + 1;
      for (int index = num - k; num < subArrayA.Length && index < subArrayB.Length && subArrayA[num + 1].CompareTo(subArrayB[index + 1]) == 0; ++index)
        ++num;
      vectorForward[k] = num;
      return num;
    }

    private void GetLcs(SubArray<T> subArrayA, SubArray<T> subArrayB, List<T> output)
    {
      if (subArrayA.Length <= 0 || subArrayB.Length <= 0)
        return;
      int pathStartX;
      int pathEndX;
      int pathK;
      int middleSnake = this.FindMiddleSnake(subArrayA, subArrayB, out pathStartX, out pathEndX, out pathK);
      int length = pathStartX - pathK;
      int num = pathEndX - pathK;
      if (middleSnake > 1)
      {
        this.GetLcs(new SubArray<T>(subArrayA, 1, pathStartX), new SubArray<T>(subArrayB, 1, length), output);
        for (int index = pathStartX + 1; index <= pathEndX; ++index)
          output.Add(subArrayA[index]);
        this.GetLcs(new SubArray<T>(subArrayA, pathEndX + 1, subArrayA.Length - pathEndX), new SubArray<T>(subArrayB, num + 1, subArrayB.Length - num), output);
      }
      else if (subArrayB.Length > subArrayA.Length)
      {
        for (int index = 1; index <= subArrayA.Length; ++index)
          output.Add(subArrayA[index]);
      }
      else
      {
        for (int index = 1; index <= subArrayB.Length; ++index)
          output.Add(subArrayB[index]);
      }
    }

    private void GetMatchPoints(
      SubArray<T> subArrayA,
      SubArray<T> subArrayB,
      List<MyersDiff<T>.Point> matchPoints)
    {
      if (subArrayA.Length <= 0 || subArrayB.Length <= 0)
        return;
      int pathStartX;
      int pathEndX;
      int pathK;
      int middleSnake = this.FindMiddleSnake(subArrayA, subArrayB, out pathStartX, out pathEndX, out pathK);
      int length1 = pathStartX - pathK;
      int num = pathEndX - pathK;
      if (middleSnake > 1)
      {
        this.GetMatchPoints(new SubArray<T>(subArrayA, 1, pathStartX), new SubArray<T>(subArrayB, 1, length1), matchPoints);
        for (int index = pathStartX + 1; index <= pathEndX; ++index)
          matchPoints.Add(new MyersDiff<T>.Point(index + subArrayA.Offset, index - pathK + subArrayB.Offset));
        this.GetMatchPoints(new SubArray<T>(subArrayA, pathEndX + 1, subArrayA.Length - pathEndX), new SubArray<T>(subArrayB, num + 1, subArrayB.Length - num), matchPoints);
      }
      else
      {
        int length2 = subArrayA.Length;
        int length3 = subArrayB.Length;
        if (length3 > length2)
        {
          int index1 = 1;
          for (int index2 = 1; index2 <= length2; ++index2)
          {
            if (subArrayA[index2].CompareTo(subArrayB[index1]) != 0)
              ++index1;
            matchPoints.Add(new MyersDiff<T>.Point(index2 + subArrayA.Offset, index1 + subArrayB.Offset));
            ++index1;
          }
        }
        else
        {
          int index1 = 1;
          for (int index2 = 1; index2 <= length3; ++index2)
          {
            if (subArrayA[index1].CompareTo(subArrayB[index2]) != 0)
              ++index1;
            matchPoints.Add(new MyersDiff<T>.Point(index1 + subArrayA.Offset, index2 + subArrayB.Offset));
            ++index1;
          }
        }
      }
    }

    private int GetReverseDPaths(
      SubArray<T> subArrayA,
      SubArray<T> subArrayB,
      int d,
      int k,
      int delta)
    {
      DiagonalVector vectorReverse = this.vectorReverse;
      int index1 = k + delta;
      int index2 = k == -d || k != d && vectorReverse[index1 + 1] <= vectorReverse[index1 - 1] ? vectorReverse[index1 + 1] - 1 : vectorReverse[index1 - 1];
      for (int index3 = index2 - index1; index2 > 0 && index3 > 0 && subArrayA[index2].CompareTo(subArrayB[index3]) == 0; --index3)
        --index2;
      vectorReverse[index1] = index2;
      return index2;
    }

    private bool SetupFictitiousPoints(SubArray<T> subArrayA, SubArray<T> subArrayB)
    {
      bool flag = false;
      if (subArrayA.Length > 0 && subArrayB.Length > 0)
      {
        this.vectorForward[1] = 0;
        int length1 = subArrayA.Length;
        int length2 = subArrayB.Length;
        this.vectorReverse[length1 - length2 + 1] = length1 + 1;
        flag = true;
      }
      return flag;
    }

    private struct Point
    {
      public Point(int x, int y)
        : this()
      {
        this.X = x;
        this.Y = y;
      }

      public int X { get; }

      public int Y { get; }
    }
  }
}