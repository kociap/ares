# Project Ares

- top-down
  limit to 4 directions for now.
- joystick left side, buttons right side
- inventory button
- skill tree
- item pickup
- movement based on a custom controller using physics collisions and collider
  casting.
  controller has to expose a function, e.g. MoveTo. gameplay logic will handle
  movement.
- heroes of hammerwatch uses a line to indicate direction of attack
- path finding with A* or any other algorithm
- camera
- music
- ai

# Workflow
Move tasks between categories freely. Once a task ends up in 'Done', it is
considered finished. If any issues arise, create new tasks to address them.
After a pull request is created, include the link to it in the task, i.e.
`Review link: <link>`

Feel free to push any changes to your own branches. Review branches must be
named after the task they address, e.g. `ares-1`. If you need a branch for
testing, `sandbox/<owner>/<branch name>`.

# Commit Format
Commit messages must be at most 80 colums wide and use the following format
```
ARES-X: Short description

Detailed description of what has been done, changed, addressed.
```
Example:
```
ARES-1: Create character movement script

Created a character script that allows a player to move their character around
the world. Added 'movement_vertical' and 'movement_horizontal' axes for keyboard
and gamepad input.
```
