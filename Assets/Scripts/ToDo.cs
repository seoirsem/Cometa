// This is just a log of things to do.

// 1. Asteroid splitting when a certain fraction of the square mesh is empty (would look cool + minimises 'wasteful' meshes) DONE
//                          
//                                              ____        ____
//                                             |    |      | xx |
//              _________                      |   x|      |xx  |
//             |       xx|                     ------      ------
//             |    xxx  |        ---->         ____        ____ 
//             |  xx     |                     |  xx|      |    |
//             |xx       |                     |xx  |      |    |
//             -----------                     ------      ------
//                                              
//
//
//
//
//
//
// 2. I get some nulls when an asteroid gets destroed completely. It looks like the derived asteroid tries to FindOutline, but the  DONE
// Main asteroids has no squares that are non-null so it hits the "Could not find a starting square!" print out in SquareMesh. Fix!