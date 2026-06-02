import os
import math

def has_collision_on_path(x, y, vx, vy, first_points, second_points):
    steps = max(abs(vx), abs(vy))
    if steps == 0:
        return any((px + x, py + y) in first_points for px, py in second_points)
        
    for step in range(steps + 1):
        cx = x + int(round(vx * step / steps))
        cy = y + int(round(vy * step / steps))
        if any((px + cx, py + cy) in first_points for px, py in second_points):
            return True
    return False

def main():
    ship_points = {(0,0), (0,1), (0,2), (1,0), (1,1), (1,2), (2,0), (2,1), (2,2)}
    torpedo_points = {(0,0)}

    relative_xs = range(-10, 10)
    relative_ys = range(-10, 10)
    relative_vxs = range(-5, 5)
    relative_vys = range(-5, 5)

    collision_states = []

    for x in relative_xs:
        for y in relative_ys:
            for vx in relative_vxs:
                for vy in relative_vys:
                    if has_collision_on_path(x, y, vx, vy, ship_points, torpedo_points):
                        collision_states.append(f"{x},{y},{vx},{vy}")

    output_dir = os.path.dirname(__file__)
    output_path = os.path.join(output_dir, "ship_torpedo_collision.txt")
    
    with open(output_path, "w") as f:
        f.write("\n".join(collision_states))
        
    print(f"Сгенерировано {len(collision_states)} состояний коллизий")
    print(f"Файл сохранен: {output_path}")

if __name__ == "__main__":
    main()