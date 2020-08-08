# Stupid.SortOfSoftBody [![License](https://img.shields.io/badge/license-MIT-orange.svg?style=flat)](LICENSE) [![http://stupidplusplus.com](https://img.shields.io/badge/Contact%20me-Here-lightgrey)](http://stupidplusplus.com)
`Turning things to jelly for your pleasure`



## First off...
#### What is this?
In unity we have the rigidbody component. It provides an object with physics simulation. There's also something called a softbody, which is basically a rigidbody but jelly. This is simply a script that will allow you to turn your rigid objects to jelly.

#### How do I use SortOfSoftBody?
Like the built in rigidbody SortOfSoftBody is a component. Simply add it to an object and tinker with the settings a little.

## Demo
If you'd like to see a demo of SortOfSoftbody simply visit [this link](http://stupidplusplus.com/gamedev.html) to play the web build (not available on mobile).

Otherwise you can always just take a look at the example scene from the unity package.

## Limitiations
Okay so on one hand playing with this script is pretty fun. On the other hand its code is stupid, dumb and not very optimized. 

SortOfSoftBody does support deforming mesh colliders along with the visual mesh but it's really only recommended for GameObjects that don't have a rigidbody or have a kinematic rigidbody. And even then it's not super performant.

Also be careful when enabling mesh deformation on GameObjects that use a non-kinematic rigidbody because, since rigidbodies require a convex mesh collider, the mesh collider will constantly recalculate itself. Which means an enourmous hit for performance.

## Future plans
If I do gain interest in softbodies in unity again I will most likely, to solve the last limitation mentioned, write a custom collider script to make things more performant.

## Credit
This code is largely just an edited version of [CatLikeCoding's mesh deformation system](https://catlikecoding.com/unity/tutorials/mesh-deformation/). CatLikeCoding makes great tutorials btw I highly recommend you check him out.

Alright well have fun playing with this if you do. Peace out✌🏻!