## UI Defragmentation and Batching

UI layout can have a big impact on Unity's ability to batch renderers. Unity's batcher must respect the hierarchy to avoid breaking the render order. Non-overlapping UI elements can be batched by material. But tiny pixel overlaps can easily break this batching! At which point, the order matters a lot. 

I used a simple script to "defragment" a given UI panel and it's child elements. Looking at the element's rect transform and atlas, we can re-order elements to improve batching.  Before re-ordering the elements, we see over 100 draw calls:

<p align="center">
  <img width="100%" src=".github/images/fragmented.gif" alt="Defragmented">
</p>


After a simple re-ordering, we can draw all sprites with just 2 draw calls:
<p align="center">
  <img width="100%" src=".github/images/de-fragmented.gif" alt="Fragmented">
</p>

The defragmentation script in this project is only meant for demonstration purposes. For simpler layouts, you can manually optimize your canvases by consequetively ordering UI elements sharing the same sprite atlas. You can also scale the rect transform of certain elements to avoid tiny pixel overlaps. This can be automated with scripting for more complex layouts.
