# Manialink z-index: why it breaks and how to avoid it

This note explains how ManiaPlanet orders overlapping elements in a `version="3"` manialink, why the small icon buttons in [MultiplayerMenu.xml](Envimix/Media/Manialinks/Universe2/MultiplayerMenu.xml) (mode help, Discord, session details) were drawn in the wrong order, and the rules to follow so it doesn't happen again.

The engine details come from reverse engineering `ManiaPlanetLogs.exe` in Ghidra. Function names are the engine's own symbols.

## The broken pattern

Each of the three buttons was built like this:

```xml
<frame pos="33 -18.5">
    <quad z-index="0" style="Bgs1" substyle="BgCardList" .../>                            <!-- border -->
    <quad z-index="0" style="Bgs1" substyle="BgCardInventoryItem" scriptevents="1" .../>  <!-- clickable fill -->
    <quad z-index="1" image="file://Media/Images/Discord.png" .../>                       <!-- icon -->
</frame>
```

This layout has two problems:

1. The border and the clickable fill share `z-index="0"` and overlap completely, so their order comes from a tie-break rule (below) and not from `z-index`.
2. The buttons sat one frame level deeper than the big menu buttons, so their layers were only about 0.00023 apart in depth. Giving them distinct z-indexes didn't bring the icon in front. The big menu buttons use the same quad + quad + label pattern and draw correctly; their layers are about 10× further apart. See [Depth gaps that are too small](#depth-gaps-that-are-too-small-why-the-discord-icon-stayed-behind).

The Discord fill's old `opacity=".1"` was a workaround: the icon was drawn behind the fill, and making the fill nearly transparent let it show through.

## How the engine turns z-index into draw order

`z-index` in ML v3 is **not a depth value**. It is only a **sort key among siblings**. The actual depth comes from the element's position in that sorted list.

1. **Sorting children** (`CGameManialinkFrame::AddChild`). Each frame keeps a second child list, `m_Childs_ZOrdered`, sorted by z-index from **highest to lowest**. A new child is inserted before the first sibling whose z-index is strictly lower. That means it goes **after** every sibling with the same z-index.

2. **Giving each child a depth slot** (`CGameManialinkFrame::PlaceChild`, `RefreshChildsZOrder`). The frame splits its depth range `[min, max]` into equal slots:

   ```
   step  = (max - min) / childCount
   depth = min + index * step
   ```

   A child **frame** gets its own range `[0, step - ε]` and splits it among its children the same way. Each frame level therefore divides the available depth by its number of children.

3. **The root range**. The page's main frame gets `[ZMin, ZMax]` from `SGameManialinkPage_BuildParams`, which defaults to **[-0.8, 0.8]**.

4. **Drawing** (`CVisionViewport::RenderDelayedsInOverlay`). Overlay visuals are radix-sorted by a float key and drawn in that order.

The highest z-index is at index 0, which gets the smallest depth, and the highest z-index is drawn in front. So **smaller depth means in front**. Together with step 1, this gives the rule that matters:

> **When siblings share a z-index, the one that comes *earlier* in the XML is drawn in front.**

HTML does the opposite: there, the later element is on top. Designing by HTML habits therefore puts the wrong element on top.

### Script changes make ties worse

When a script sets `Control.ZIndex`, `CGameManialinkFrame::ChangeChildZOrder` moves the control to its new place in the sorted list. It also goes after any siblings with the same value. After a few runtime changes, the order among tied siblings depends on what happened before, not on the XML. A layout that relies on ties can look right at load and wrong later.

### Depth gaps that are too small (why the Discord icon stayed behind)

Every frame level divides the depth range again. For the Discord button, in its original place inside `FrameMultiplayer`, the chain was roughly:

| Level | Children | Slot size |
|---|---|---|
| root `[-0.8, 0.8]` | 2 | 0.8 |
| `<frame z-index="10">` | 7 | 0.114 |
| `FrameMenu` | 1 | 0.114 |
| `<frame pos="-110 0">` | 15 | 0.0076 |
| `FrameMultiplayer` | 11 | 0.00069 |
| `FrameButtonServerDetails` | 3 | 0.00023 |

The layers inside the button were therefore about **0.00023** apart. The big menu buttons (Continue, Spectator, …) sit one level higher. Their layers are about **0.0025** apart, and they draw correctly with the same quad + quad + label pattern.

The engine also uses **fixed-size** depth constants that don't shrink with the slots. For example, `CControlFrame::AddChild` nudges any child placed at exactly depth 0 by **1e-4**, and the first child of every frame starts at 0. Once siblings are only a few 1e-4 apart, offsets like this are as large as the gap itself and can swap layers. This is the most likely explanation, but it is inferred. These are the observations it rests on:

- Distinct z-indexes (0/1/2) did not bring the icon in front.
- Dropping the icon's opacity below 1 did not either. The overlay renders opaque visuals (`DelayedToSortOpaques`) before the depth-sorted list, which made a full-opacity image quad a suspect, but it wasn't the cause.
- The identical pattern works one level up, where the gap is about 10× larger.

The engine code that applies the offset during drawing wasn't traced.

Two more thresholds matter for deep or wide trees:

- `CGameManialinkControl::SetZOrderedPosZ` **ignores a depth change smaller than 1e-5**. When the order is recalculated at runtime (after a script `ZIndex` change, for example), children whose slot moves by less than that keep their old depth.
- `PlaceChild` and `RefreshChildsZOrder` log `[CControlFrame] Range below epsilon` when a slot drops below 1e-20.

**Fix:** move layered widgets up the tree, or into frames with few siblings, so their layers get larger depth gaps. A frame's slot size is its parent's slot divided by the parent's child count, so every level and every sibling counts.

## Rules

1. **Give every overlapping sibling its own z-index.** Never let two elements that cover each other share a value. Use the order you want: background, then interactive layer, then glyph or icon.
2. **Don't rely on XML order to break ties.** It is reversed compared to HTML, and script `ZIndex` changes can alter it.
3. **z-index only matters between siblings.** A child can't rise above its parent frame's neighbours, however high its value. To put something above another subtree, change the z-index of the frames that contain them.
4. **Keep layered widgets shallow in the frame tree.** If a button stacks a background, a clickable quad and a glyph, keep it at a level where the depth gaps are large. Avoid nesting it several frames deep under parents with many children. As a guide, the working big buttons have a gap of about 0.0025 and the broken small ones had about 0.00023.
5. **Keep the tree narrow where you reorder at runtime.** Each extra child shrinks every sibling's slot. Group rarely-moving content into frames rather than adding many direct siblings.
6. **Don't hide z-order problems by lowering the opacity of the element in front.** If something has to be nearly transparent to show what's under it, check the z-indexes, then the depth gap (rule 4).

## The fix applied

In [MultiplayerMenu.xml](Envimix/Media/Manialinks/Universe2/MultiplayerMenu.xml):

- The three button frames (mode help, `FrameButtonServerDetails`, `FrameButtonSessionDetails`) moved out of `FrameMultiplayer` into its parent, the same level as the big menu buttons. Their positions now include `FrameMultiplayer`'s `0 -35` offset (`33 -46.5`, `33 -53.5`, `33 -60.5`), so they stay in the same place on screen. Nothing in the script depended on their parent.
- Each button uses distinct z-indexes:

  ```xml
  <quad  z-index="0" substyle="BgCardList" .../>                            <!-- border -->
  <quad  z-index="1" substyle="BgCardInventoryItem" scriptevents="1" .../>  <!-- clickable fill -->
  <quad  z-index="2" image="file://Media/Images/Discord.png" .../>          <!-- icon / glyph -->
  ```

- The Discord fill's starting opacity is back to `.5` to match the other two buttons. `UpdateEnvimaniaSessionButtons()` still sets it from script at runtime.

[Menu.xml](Envimix/Media/Manialinks/Universe2/Menu.xml) has the same small buttons nested inside its `FrameMultiplayer`, so the same problem probably applies there. Its `FrameMultiplayer` is shown or hidden from script (`FrameMultiplayer.Visible = !IsSolo()`), so buttons moved out of it would need their own visibility toggle.
