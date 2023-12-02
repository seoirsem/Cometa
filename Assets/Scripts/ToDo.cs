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

// 3. Do NOT use the SimplePool for asteroids. If a 'second hand' asteroid object gets re-used, sometimes the dictionaries and other internal data 
// from an older incarnation gets used again in, and this is not good - causes old shapes/vertices etc. to be used, leading to jittery visuals!

// 4. I think the issue where when an astaroid gets split, one of the chunks goes invisible, is still there. I think this is a corner case where
// the ouytline finidng algorithm gets a bit confused because of a peculiar startin point (it starts finding the outline from an isolated square 
// and it finishes too soon). I think it's something to do with the cracking algo that makes the crack edges look 'natural' by randomly removing some
// of the squares along the edge. See below, if x's get removed and A doesn't, outline algo starting from A will see A as the only square in the asteroid
// and conclude the search at A.

//                                            
//                                           
//              _________    
//             |         |        
//             |x        |                   
//             |A x      |                
//             -----------                   