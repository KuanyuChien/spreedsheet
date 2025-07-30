// Skeleton implementation written by Joe Zachary for CS 3500, September 2013
// Version 1.1 - Joe Zachary
// (Fixed error in comment for RemoveDependency)
// Version 1.2 - Daniel Kopta Fall 2018
// (Clarified meaning of dependent and dependee)
// (Clarified names in solution/project structure)
// Version 1.3 - H. James de St. Germain Fall 2024
// Co-Author: Hung Nguyen, Date: 9/15/2024, Course: CS 3500
namespace CS3500.DependencyGraph;

/// <summary>
/// <para>
/// (s1,t1) is an ordered pair of strings, meaning t1 depends on s1.
/// (in other words: s1 must be evaluated before t1.)
/// </para>
/// <para>
/// A DependencyGraph can be modeled as a set of ordered pairs of strings.
/// Two ordered pairs (s1,t1) and (s2,t2) are considered equal if and only
/// if s1 equals s2 and t1 equals t2.
/// </para>
/// <remarks>
/// Recall that sets never contain duplicates.
/// If an attempt is made to add an element to a set, and the element is already
/// in the set, the set remains unchanged.
/// </remarks>
/// <para>
/// Given a DependencyGraph DG:
/// </para>
/// <list type="number">
/// <item>
/// If s is a string, the set of all strings t such that (s,t) is in DG is called dependents(s).
/// (The set of things that depend on s.)
/// </item>
/// <item>
/// If s is a string, the set of all strings t such that (t,s) is in DG is called dependees(s).
/// (The set of things that s depends on.)
/// </item>
/// </list>
/// <para>
/// For example, suppose DG = {("a", "b"), ("a", "c"), ("b", "d"), ("d","d")}.
/// </para>
/// <code>
/// dependents("a") = {"b", "c"}
/// dependents("b") = {"d"}
/// dependents("c") = {}
/// dependents("d") = {"d"}
/// dependees("a") = {}
/// dependees("b") = {"a"}
/// dependees("c") = {"a"}
/// dependees("d") = {"b", "d"}
/// </code>
/// </summary>
public class DependencyGraph
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DependencyGraph"/> class.
    /// The initial DependencyGraph is empty.
    /// </summary>
    Dictionary<String, HashSet<String>> dependeesDictionary;    // A dictionary/hashmap to keep track of sets of dependees using each dependent as key
    Dictionary<String, HashSet<String>> dependentsDictionary;    // A dictionary/hashmap to keep track of sets of dependents using each dependee as key
    int size;
    public DependencyGraph()
    {
        dependeesDictionary = new Dictionary<String, HashSet<String>>();
        dependentsDictionary = new Dictionary<String, HashSet<String>>();
        size = 0;
    }

    /// <summary>
    /// The number of ordered pairs in the DependencyGraph.
    /// </summary>
    public int Size
    {
        get { return size; }
    }

    /// <summary>
    /// Reports whether the given node has dependents (i.e., other nodes depend on it).
    /// </summary>
    /// <param name="nodeName"> The name of the node.</param>
    /// <returns> true if the node has dependents. </returns>
    public bool HasDependents(string nodeName)
    {
        return dependentsDictionary.ContainsKey(nodeName);
    }

    /// <summary>
    /// Reports whether the given node has dependees (i.e., depends on one or more other nodes).

    /// </summary>
    /// <returns> true if the node has dependees.</returns>
    /// <param name="nodeName">The name of the node.</param>
    public bool HasDependees(string nodeName)
    {
        return dependeesDictionary.ContainsKey(nodeName);
    }

    /// <summary>
    /// <para>
    /// Returns the dependents of the node with the given name.
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependents of nodeName. </returns>
    public IEnumerable<string> GetDependents(string nodeName)
    {
        if (HasDependents(nodeName)) { return dependentsDictionary[nodeName]; } //Check if key exist, if it does return the hashset under that key
        return new HashSet<string>(); // Return empty set if nothing exist under that key
    }

    /// <summary>
    /// <para>
    /// Returns the dependees of the node with the given name.
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The node we are looking at.</param>
    /// <returns> The dependees of nodeName. </returns>
    public IEnumerable<string> GetDependees(string nodeName)
    {
        if (HasDependees(nodeName)) { return dependeesDictionary[nodeName]; } //Check if key exist, if it does return the hashset under that key
        return new HashSet<string>(); // Return empty set if nothing exist under that key
    }

    /// <summary>
    /// <para>Adds the ordered pair (dependee, dependent), if it doesn't exist.</para>
    /// <para>
    /// This can be thought of as: dependee must be evaluated before dependent
    /// </para>
    /// </summary>
    /// <param name="dependee"> the name of the node that must be evaluated first</param>
    /// <param name="dependent"> the name of the node that cannot be evaluated until after dependee</param>
    public void AddDependency(string dependee, string dependent)
    {
        HashSet<string> dependencies;
        if (!dependeesDictionary.ContainsKey(dependent) || !dependentsDictionary.ContainsKey(dependee)) { size++; } // If dependency hasn't existed in the dictionary, increase pair of dependency
        // Add dependee to dependent key
        if (HasDependees(dependent)) // If Dependency Graph has dependees under key dependent
        {
            dependencies = dependeesDictionary[dependent];                         // Try to get list under key dependee
            if (!dependencies.Contains(dependee))                                   // If there is no dependee under the dependents, add it to the set
            {
                dependencies.Add(dependee);
            }
        }
        else // Else no dependees under key dependent
        {
            dependencies = new HashSet<string>();    // New Hashset
            dependencies.Add(dependee);
            dependeesDictionary.Add(dependent, dependencies);  // Add new hashset under new key
        }
        // Add dependent to dependee key
        if (HasDependents(dependee)) // If Dependency Graph has dependents under key dependee
        {
            dependencies = dependentsDictionary[dependee];                           // Try to get list under key dependent
            if (!dependencies.Contains(dependent))                                  // If there is no dependent under the dependee, add it to the list and increase size
            {
                dependencies.Add(dependent);
            }
        }
        else // Else no dependents under key dependee
        {
            dependencies = new HashSet<string>();    // New Hashset
            dependencies.Add(dependent);
            dependentsDictionary.Add(dependee, dependencies);    // Add new hashset under new key
        }

    }
    /// <summary>
    /// <para>
    /// Removes the ordered pair (dependee, dependent), if it exists.
    /// </para>
    /// </summary>
    /// <param name="dependee"> The name of the node that must be evaluated first</param>

    /// <param name="dependent"> The name of the node that cannot be evaluated until after dependee</param>
    public void RemoveDependency(string dependee, string dependent)
    {
        if (!dependentsDictionary.ContainsKey(dependee) || !dependeesDictionary.ContainsKey(dependent)) { return; } // End the method if keys aren't found in both sets of dependencies
        else
        {
            if (dependeesDictionary.ContainsKey(dependent) && dependentsDictionary.ContainsKey(dependee)) { size--; }  // If dependency has existed in the dictionary, decrease pair of dependency
            /// Remove dependee link to dependent
            if (HasDependees(dependent))
            {
                dependeesDictionary[dependent].Remove(dependee);
                if (dependeesDictionary[dependent].Count == 0) { dependeesDictionary.Remove(dependent); }    // Remove dependee key if the set under it is empty
            }


            /// Remove dependent link to dependee
            if (HasDependents(dependee))
            {
                dependentsDictionary[dependee].Remove(dependent);
                if (dependentsDictionary[dependee].Count == 0) { dependentsDictionary.Remove(dependee); } // Remove dependee key if the set under it is empty
            }
        }
    }

    /// <summary>
    /// Removes all existing ordered pairs of the form (nodeName, *). Then, for each
    /// t in newDependents, adds the ordered pair (nodeName, t).
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependents are being replaced</param>
    /// <param name="newDependents"> The new dependents for nodeName</param>
    public void ReplaceDependents(string nodeName, IEnumerable<string> newDependents)
    {
        if (HasDependents(nodeName))
        {
            foreach (string dependent in dependentsDictionary[nodeName]) { RemoveDependency(nodeName, dependent);  } // Remove the HashSet under key nodeName 
        }
        foreach (string dependent in newDependents) { AddDependency(nodeName, dependent); } // Foreach x in newDependents, add it into the HashSet

    }

    /// <summary>
    /// <para>
    /// Removes all existing ordered pairs of the form (*, nodeName). Then, foreach
    /// t in newDependees, adds the ordered pair (t, nodeName).
    /// </para>
    /// </summary>
    /// <param name="nodeName"> The name of the node who's dependees are being replaced</param>
    /// <param name="newDependees"> The new dependees for nodeName</param>
    public void ReplaceDependees(string nodeName, IEnumerable<string> newDependees)
    {
        if (HasDependees(nodeName))
        {
            foreach (string dependee in dependeesDictionary[nodeName]) { RemoveDependency(dependee, nodeName); }  // Remove the HashSet under key nodeName 
        }
        foreach (string dependee in newDependees) { AddDependency(dependee, nodeName); } // Foreach x in newDependees, add it into the HashSet
    }
}