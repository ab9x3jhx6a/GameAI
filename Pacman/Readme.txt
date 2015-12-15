Three states for ghosts: wander, chase & runaway, depends on whether pacman ate the superpallet and how close pacman is.
Also the same three states for pacman, depends on the same factors.
Ghost will move randomly when wandering, pacman will try to pick up as many as pallets possible if wandering and move towards the super pallet.
Also while running away, pacman will try to avoid ghosts but run towards super pallet at the same time. Ghosts will try to run back to where they were spawned when chased.
It runs pretty smoothly, sometime it'll have a framerate drop due to many pathfinding running at the same time.