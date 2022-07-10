# Procedural Dungeon Generator
Individual project at 4th semester of Applied Computer Science at Warsaw Univesity of Technology. \
Project uses Unity's built-in collision simulation to position the rooms, and then Delaunay triangulation and graph algorithms to connect them. \
For more in-depth description, visit [this blog](https://www.gamedeveloper.com/programming/procedural-dungeon-generation-algorithm), since the project was built entirely around the idea presented here.

## Used libraries:
- [Triangle.NET](https://github.com/wo80/Triangle.NET) - a C# port of [Triangle](https://www.cs.cmu.edu/~quake/triangle.html) \
**Author of the original software**: [John Richard Shewchuk](https://people.eecs.berkeley.edu/~jrs/) \
**Author of the port**: [Christian Woltering](https://github.com/wo80) \
**License**: Licensing situation of this library is unclear, author of the port states the following in his project's [README](https://github.com/wo80/Triangle.NET/blob/master/README.md):
> *The original C code published by Jonathan Shewchuk comes with a proprietary license (see [Triangle README](https://github.com/wo80/Triangle/blob/master/src/Triangle/README)) which, unfortunately, isn't very clear about how a derived work like Triangle.NET should be handled. Though Triangle.NET was published on Codeplex under the MIT license in 2012 (triangle.codeplex.com, no longer available), I recommend not using this code in a commercial context. This restriction only applies to the Triangle project and specifically those files containing a copyright header pointing to Jonathan Richard Shewchuk. The code contained in the other projects (like Triangle.Rendering or Triangle.Viewer) is released under MIT license. Due to the unclear licensing situation, there will also be no Nuget package release. For further discussion, please refer to the open issue [License Confusion](https://github.com/wo80/Triangle.NET/issues/6).*

- [QuickGraph](https://github.com/YaccConstructor/QuickGraph) \
**Authors**: List of contributors is rather long, to see it, click [here](https://github.com/YaccConstructor/QuickGraph/graphs/contributors "QuickGraph contributors") \
**License**: [Microsoft Public License](https://github.com/YaccConstructor/QuickGraph/blob/master/LICENSE.txt)

## Sprites
**Source**: [opengameart.org](https://opengameart.org/content/dungeon-crawl-32x32-tiles-supplemental) \
**License**: [CC0](https://creativecommons.org/publicdomain/zero/1.0/)

## Other credits
Algorithm used for generating random convex polygons was based on [Sander Verdonschot](https://cglab.ca/~sander/)'s Java implementation available [here](), based on a publication by [Pavel Valtr](https://zbmath.org/authors/?q=ai%3Avaltr.pavelhttps://zbmath.org/authors/?q=ai%3Avaltr.pavel): [*Probability that n random points are in convex position*](https://zbmath.org/?q=an%3A0820.60007) \
Algorithm used to get centroid of a polygon is just formulas from [Wikipedia](https://en.wikipedia.org/wiki/Centroid#Of_a_polygon).