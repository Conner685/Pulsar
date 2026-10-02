# XZ gameplay convention

- Grid keys remain `Vector2Int`: their second coordinate means local Z.
- `ShipPlane` owns conversions, tile quarter-turns, constrained physics, and sprite visual creation.
- Physics roots have unit scale, fixed world Y, and Y-only rotation. The ship root's height defines the cursor plane; test-spawned parts inherit that height.
- Edge indices remain forward (+Z), right (+X), back (-Z), left (-X). Positive logical quarter-turns use negative Y rotation, preserving the existing connector indexing.
- Attached tiles have BoxColliders and share the ship Rigidbody. Floating tiles have separate Rigidbodies and trigger BoxColliders.
- Tile and ghost sprites live on visual children tilted 90 degrees about X. Gameplay roots must not receive this tilt.
- The existing URP renderer is retained. Shared unlit sprite material is included through Resources/Pulsar/SpriteUnlit; scan, tiles, and ghost retain explicit sorting orders with transparent depth writes disabled.
- Detached parts use Rigidbody.GetPointVelocity. As before, recreation resets HP and orientation; changing that behavior is outside this migration.
- Build-Destroy's ship remains kinematic. Force-driven movement requires making it dynamic; keep its planar constraints.

## Validation

From Unity, outside Play Mode, select **Pulsar > Validate XZ Migration**. This opens Build-Destroy and temporarily enters Play Mode. It checks scene configuration, all connector rotations, transformed grid lookups, attachment/ghost agreement, rotated scan containment, top-down and angled camera ray-plane picking, orphan release velocity, and constrained 3D physics. Results appear in the Console; the scene is not saved by the checks.

For an isolated project copy, run Unity with `-batchmode -nographics -projectPath <copy> -executeMethod XZMigrationChecks.Run -logFile <log>` (without `-quit`; the check exits after Play Mode validation).

Manual visual check: open the saved Build-Destroy scene, enter Play Mode, press S to spawn parts, Tab to scan, hover a part, C to attach, and X over a non-core tile to destroy it. Confirm sprites, scan fill/outline, and ghost visibility. These visual checks are not covered by headless physics validation.
