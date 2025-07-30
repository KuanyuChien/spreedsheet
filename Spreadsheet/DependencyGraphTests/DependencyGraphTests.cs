namespace CS3500.DevelopmentTests;

using CS3500.DependencyGraph;
using System.Drawing;
using System.Linq;

/// <summary>
///   This is a test class for DependencyGraphTest and is intended
///   to contain all DependencyGraphTest Unit Tests
/// </summary>
[TestClass]
public class DependencyGraphTests
{
    /// <summary>
    ///   TODO:  Explain carefully what this code tests.
    ///          Also, update in-line comments as appropriate.
    /// </summary>
    [TestMethod]
    [Timeout(2000)]  // 2 second run time limit
    public void StressTest()
    {
        DependencyGraph dg = new();

        // A bunch of strings to use
        const int SIZE = 200;
        string[] letters = new string[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            letters[i] = string.Empty + ((char)('a' + i));
        }

        // The correct answers
        HashSet<string>[] dependents = new HashSet<string>[SIZE];
        HashSet<string>[] dependees = new HashSet<string>[SIZE];
        for (int i = 0; i < SIZE; i++)
        {
            dependents[i] = [];
            dependees[i] = [];
        }

        // Add a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j++)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove a bunch of dependencies
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 4; j < SIZE; j += 4)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Add some back
        for (int i = 0; i < SIZE; i++)
        {
            for (int j = i + 1; j < SIZE; j += 2)
            {
                dg.AddDependency(letters[i], letters[j]);
                dependents[i].Add(letters[j]);
                dependees[j].Add(letters[i]);
            }
        }

        // Remove some more
        for (int i = 0; i < SIZE; i += 2)
        {
            for (int j = i + 3; j < SIZE; j += 3)
            {
                dg.RemoveDependency(letters[i], letters[j]);
                dependents[i].Remove(letters[j]);
                dependees[j].Remove(letters[i]);
            }
        }

        // Make sure everything is right
        for (int i = 0; i < SIZE; i++)
        {
            Assert.IsTrue(dependents[i].SetEquals(new HashSet<string>(dg.GetDependents(letters[i]))));
            Assert.IsTrue(dependees[i].SetEquals(new HashSet<string>(dg.GetDependees(letters[i]))));
        }
    }

    [TestMethod]
    public void Size_TestEmptyGraph_SizeIsZero()
    {
        DependencyGraph dg = new();
        Assert.AreEqual(dg.Size, 0);
    }

    [TestMethod]
    public void size_TestAddOnePair_SizeIsOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.AreEqual(dg.Size, 1);
    }

    [TestMethod]
    public void Size_TestAddTwoIndependentPair_SizeIsTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("c", "d");
        Assert.AreEqual(dg.Size, 2);
    }

    [TestMethod]
    public void Size_TestAddTwoConnectPair_SizeIsTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        Assert.AreEqual(dg.Size, 2);
    }

    [TestMethod]
    public void Size_TestLargerGraph_SizeIsTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.AddDependency("c", "d");
        dg.AddDependency("b", "e");
        dg.AddDependency("f", "a");
        dg.AddDependency("b", "c");
        dg.AddDependency("e", "d");
        dg.AddDependency("g", "c");
        dg.AddDependency("g", "d");
        Assert.AreEqual(dg.Size, 8);
    }

    [TestMethod]
    public void Size_TestAddTwoSamePair_SizeIsOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "b");
        Assert.AreEqual(dg.Size, 1);
    }

    [TestMethod]
    public void Size_TestTwoSectionGraph()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.AddDependency("b", "d");
        Assert.AreEqual(dg.Size, 3);
        dg.RemoveDependency("a", "b");
        dg.AddDependency("a", "e");
        Assert.AreEqual(dg.Size, 3);
    }

    [TestMethod]
    public void HasDependents_TestWithNoDependentNode_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsFalse(dg.HasDependents("b"));
    }

    [TestMethod]
    public void HasDependents_TestWithTwoDependentNode_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        Assert.IsTrue(dg.HasDependents("a"));
    }

    [TestMethod]
    public void HasDependents_TestWithMiddleNode_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        Assert.IsTrue(dg.HasDependents("b"));
    }

    [TestMethod]
    public void HasDependents_TestAfterRemove_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.RemoveDependency("b", "c");
        Assert.IsFalse(dg.HasDependents("b"));
    }

    [TestMethod]
    public void HasDependees_TestWithNoDependeeNode_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        Assert.IsFalse(dg.HasDependees("a"));
    }

    [TestMethod]
    public void HasDependees_TestWithTwoDependeeNode_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "c");
        Assert.IsTrue(dg.HasDependees("c"));
    }

    [TestMethod]
    public void HasDependees_TestWithMiddleNode_True()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        Assert.IsTrue(dg.HasDependees("b"));
    }

    [TestMethod]
    public void HasDependees_TestAfterRemove_False()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.RemoveDependency("a", "b");
        Assert.IsFalse(dg.HasDependees("b"));
    }

    [TestMethod]
    public void GetDependents_TestWithNoDependentNodes_EmptyHashSet()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        Assert.AreEqual(dg.GetDependents("b").Count(), 0);
        Assert.AreEqual(dg.GetDependents("c").Count(), 0);
    }

    [TestMethod]
    public void GetDependents_TestWithTwoDependentNodes_HashSetOfSizeTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "c");
        Assert.IsTrue(dg.GetDependents("a").Contains("b"));
        Assert.IsTrue(dg.GetDependents("a").Contains("c"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 2);
    }

    [TestMethod]
    public void GetDependents_TestWithMiddleNodes_HashSetOfSizeTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.AddDependency("e", "b");
        dg.AddDependency("b", "f");
        Assert.IsTrue(dg.GetDependents("b").Contains("c"));
        Assert.IsTrue(dg.GetDependents("b").Contains("f"));
        Assert.AreEqual(dg.GetDependents("b").Count(), 2);
    }

    [TestMethod]
    public void GetDependees_TestWithNoDependentNodes_EmptyHashSet()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "c");
        Assert.AreEqual(dg.GetDependees("b").Count(), 0);
        Assert.AreEqual(dg.GetDependees("a").Count(), 0);
    }

    [TestMethod]
    public void GetDependees_TestWithTwoDependentNodes_HashSetOfSizeTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "c");
        dg.AddDependency("b", "c");
        Assert.IsTrue(dg.GetDependees("c").Contains("b"));
        Assert.IsTrue(dg.GetDependees("c").Contains("a"));
        Assert.AreEqual(dg.GetDependees("c").Count(), 2);
    }

    [TestMethod]
    public void GetDependees_TestWithMiddleNodes_HashSetOfSizeTwo()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.AddDependency("e", "b");
        dg.AddDependency("b", "f");
        Assert.IsTrue(dg.GetDependees("b").Contains("a"));
        Assert.IsTrue(dg.GetDependees("b").Contains("e"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 2);
    }

    [TestMethod]
    public void AddDependency_TestAddOnePair_HashSetOfSizeOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        Assert.IsTrue(dg.GetDependents("a").Contains("b"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 1);
        Assert.IsTrue(dg.GetDependees("b").Contains("a"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 1);
        Assert.AreEqual(dg.Size, 1);
    }

    [TestMethod]
    public void AddDependency_TestAddDuplicatePair()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "b");
        dg.AddDependency("a", "b");
        Assert.AreEqual(dg.Size, 1);
        Assert.IsTrue(dg.GetDependents("a").Contains("b"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 1);
        Assert.IsTrue(dg.GetDependees("b").Contains("a"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 1);
    }

    [TestMethod]
    public void RemoveDependency_TestRemoveOnePair_HashSetOfSizeOne()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.RemoveDependency("a", "b");
        Assert.IsFalse(dg.GetDependents("a").Contains("b"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 0);
        Assert.IsFalse(dg.GetDependees("b").Contains("a"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 0);
        Assert.AreEqual(dg.Size, 1);
    }

    [TestMethod]
    public void RemoveDependency_TestRemoveDuplicatePair()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        dg.RemoveDependency("a", "b");
        dg.RemoveDependency("a", "b");
        Assert.IsFalse(dg.GetDependents("a").Contains("b"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 0);
        Assert.IsFalse(dg.GetDependees("b").Contains("a"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 0);
        Assert.AreEqual(dg.Size, 1);
    }

    [TestMethod]
    public void ReplaceDependents_TestReplaceWithNoDependentNode()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        HashSet<string> replace = new();
        replace.Add("x");
        replace.Add("y");
        replace.Add("z");
        dg.ReplaceDependents("b", replace);
        Assert.IsFalse(dg.GetDependents("a").Contains("x"));
        Assert.IsTrue(dg.GetDependents("a").Contains("b"));
        Assert.IsTrue(dg.GetDependents("b").Contains("x"));
        Assert.IsTrue(dg.GetDependents("b").Contains("y"));
        Assert.IsTrue(dg.GetDependents("b").Contains("z"));
        Assert.AreEqual(dg.GetDependents("b").Count(), 3);
        Assert.AreEqual(dg.Size, 4);
    }

    [TestMethod]
    public void ReplaceDependents_TestReplaceWithTwoPairGraph()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        HashSet<string> replace = new();
        replace.Add("x");
        replace.Add("y");
        replace.Add("z");
        dg.ReplaceDependents("a", replace);
        Assert.IsFalse(dg.GetDependents("a").Contains("b"));
        Assert.IsTrue(dg.GetDependents("a").Contains("x"));
        Assert.IsTrue(dg.GetDependents("a").Contains("y"));
        Assert.IsTrue(dg.GetDependents("a").Contains("z"));
        Assert.IsTrue(dg.GetDependents("b").Contains("c"));
        Assert.AreEqual(dg.GetDependents("a").Count(), 3);
        Assert.AreEqual(dg.Size, 4);
    }


    [TestMethod]
    public void ReplaceDependees_TestReplaceWithNoDependeeNode()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        HashSet<string> replace = new();
        replace.Add("x");
        replace.Add("y");
        replace.Add("z");
        dg.ReplaceDependees("a", replace);
        Assert.IsTrue(dg.GetDependents("a").Contains("b"));
        Assert.IsTrue(dg.GetDependees("a").Contains("x"));
        Assert.IsTrue(dg.GetDependees("a").Contains("y"));
        Assert.IsTrue(dg.GetDependees("a").Contains("z"));
        Assert.AreEqual(dg.GetDependees("a").Count(), 3);
        Assert.AreEqual(dg.Size, 4);
    }

    [TestMethod]
    public void ReplaceDependees_TestReplaceWithTwoPairGraph()
    {
        DependencyGraph dg = new();
        dg.AddDependency("a", "b");
        dg.AddDependency("b", "c");
        HashSet<string> replace = new();
        replace.Add("x");
        replace.Add("y");
        replace.Add("z");
        dg.ReplaceDependees("b", replace);
        Assert.IsFalse(dg.GetDependees("a").Contains("b"));
        Assert.IsTrue(dg.GetDependees("b").Contains("x"));
        Assert.IsTrue(dg.GetDependees("b").Contains("y"));
        Assert.IsTrue(dg.GetDependees("b").Contains("z"));
        Assert.IsTrue(dg.GetDependents("b").Contains("c"));
        Assert.AreEqual(dg.GetDependees("b").Count(), 3);
        Assert.AreEqual(dg.Size, 4);
    }
}