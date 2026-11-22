"""Copy location art from the drop folder into the game as plates, renamed to their tags.

The art is drawn and dropped under Images/Locations/<place>/ with the names it was made
under (inside_night.png, outside_day_snow.png); the game wants each one under its plate
tag (kasia_bar_night.png, kasia_front_day_winter.png). SOURCES below is that mapping.
The files are copied as they are: the game imports them as sprites itself (ArtImporter).

    python3 tools/import_plates.py               # Images/    -> Resources/Plates
    python3 tools/import_plates.py --style alt   # ImagesAlt/ -> Resources/Styles/alt/Plates

A second art style's drop folder mirrors Images/ (ImagesAlt/ for "alt"), and only needs
the pictures redrawn so far: the game shows the base art for the rest (ArtStyles). Files
already up to date are left alone; a file with no entry in SOURCES is listed, so a new
picture gets its tag added here before it goes in. Run it with --dry-run to see what
would change.
"""

import argparse
import filecmp
import shutil
from pathlib import Path

ROOT = Path(__file__).resolve().parent.parent

# Drop-folder path under Locations/ -> plate tag.
SOURCES = {
    "MillstoneLanes/inside.png": "lanes_lanes_night",
    "MillstoneLanes/dice_background_landscape.png": "lanes_circle_night_wide",
    "MillstoneLanes/dice_background_portrait.png": "lanes_circle_night",
    "MillstoneLanes/outside_day.png": "lanes_front_day",
    "MillstoneLanes/outside_day_rain.png": "lanes_front_day_rain",
    "MillstoneLanes/outside_day_snow.png": "lanes_front_day_winter",
    "MillstoneLanes/outside_night.png": "lanes_front_night",
    "MillstoneLanes/outside_night_rain.png": "lanes_front_night_rain",
    "MillstoneLanes/outside_night_snow.png": "lanes_front_night_winter",
    "MillstoneLanes/outside_rear_lot_day.png": "lanes_backlot_day",
    "MillstoneLanes/outside_rear_lot_day_rain.png": "lanes_backlot_day_rain",
    "MillstoneLanes/outside_rear_lot_day_snow.png": "lanes_backlot_day_winter",
    "MillstoneLanes/outside_rear_lot_night.png": "lanes_backlot_night",
    "MillstoneLanes/outside_rear_lot_night_rain.png": "lanes_backlot_night_rain",
    "MillstoneLanes/outside_rear_lot_night_snow.png": "lanes_backlot_night_winter",
    "StarFlare1/bar.png": "flare_bar_night",
    "StarFlare1/bar_closed.png": "flare_bar_night_decline",
    "StarFlare1/floor.png": "flare_floor_night",
    "StarFlare1/floor_closed.png": "flare_floor_night_decline",
    "StarFlare1/inside_gus_office_day.png": "flare_office_day",
    "StarFlare1/inside_gus_office_day_rain.png": "flare_office_day_rain",
    "StarFlare1/inside_gus_office_day_snow.png": "flare_office_day_winter",
    "StarFlare1/inside_gus_office_night.png": "flare_office_night",
    "StarFlare1/inside_gus_office_night_rain.png": "flare_office_night_rain",
    "StarFlare1/inside_gus_office_night_snow.png": "flare_office_night_winter",
    "StarFlare1/lounge.png": "flare_lounge_night",
    "StarFlare1/lounge_closed.png": "flare_lounge_night_covered",
    "StarFlare1/outside_condemned.png": "flare_front_condemned",
    "StarFlare1/outside_day.png": "flare_front_day",
    "StarFlare1/outside_day_burned.png": "flare_burned_day",
    "StarFlare1/outside_day_rain.png": "flare_front_day_rain",
    "StarFlare1/outside_day_snow.png": "flare_front_day_winter",
    "StarFlare1/outside_night.png": "flare_front_night",
    "StarFlare1/outside_night_rain.png": "flare_front_night_rain",
    "StarFlare1/outside_night_snow.png": "flare_front_night_winter",
    "StarFlare1/outside_predawn-closed.png": "flare_front_dawn_covered",
    "StarFlare1/outside_predawn_truck.png": "flare_front_dawn_flatbed",
    "bridge_head_fuel/inside_day.png": "bridgehead_store_day",
    "bridge_head_fuel/inside_day_rain.png": "bridgehead_store_day_rain",
    "bridge_head_fuel/inside_day_snow.png": "bridgehead_store_day_winter",
    "bridge_head_fuel/inside_night.png": "bridgehead_store_night",
    "bridge_head_fuel/inside_night_rain.png": "bridgehead_store_night_rain",
    "bridge_head_fuel/inside_night_snow.png": "bridgehead_store_night_winter",
    "bridge_head_fuel/outside_day.png": "bridgehead_forecourt_day",
    "bridge_head_fuel/outside_day_rain.png": "bridgehead_forecourt_day_rain",
    "bridge_head_fuel/outside_day_snow.png": "bridgehead_forecourt_day_winter",
    "bridge_head_fuel/outside_night.png": "bridgehead_forecourt_night",
    "bridge_head_fuel/outside_night_rain.png": "bridgehead_forecourt_night_rain",
    "bridge_head_fuel/outside_night_snow.png": "bridgehead_forecourt_night_winter",
    "kasias_tap/back_room.png": "kasia_backroom_night",
    "kasias_tap/inside_day.png": "kasia_bar_day",
    "kasias_tap/inside_day_rain.png": "kasia_bar_day_rain",
    "kasias_tap/inside_day_snow.png": "kasia_bar_day_winter",
    "kasias_tap/inside_night.png": "kasia_bar_night",
    "kasias_tap/inside_night_rain.png": "kasia_bar_night_rain",
    "kasias_tap/inside_night_snow.png": "kasia_bar_night_winter",
    "kasias_tap/outside_day.png": "kasia_front_day",
    "kasias_tap/outside_day_rain.png": "kasia_front_day_rain",
    "kasias_tap/outside_day_snow.png": "kasia_front_day_winter",
    "kasias_tap/outside_night.png": "kasia_front_night",
    "kasias_tap/outside_night_rain.png": "kasia_front_night_rain",
    "kasias_tap/outside_night_snow.png": "kasia_front_night_winter",
    "kasias_tap/upstairs-wedding.png": "kasia_wedding_night",
    "kasias_tap/upstairs_wedding.png": "kasia_wedding_night",
    "kasias_tap/upstairs_wedding_day.png": "kasia_wedding_day",
    "prairie-queen/inside_bar.png": "pq_bar_night",
    "prairie-queen/inside_blackjack_table.png": "pq_table4_night",
    "prairie-queen/inside_day.png": "pq_salon_day",
    "prairie-queen/inside_day_rain.png": "pq_salon_day_rain",
    "prairie-queen/inside_floor_night.png": "pq_salon_night",
    "prairie-queen/inside_floor_rain.png": "pq_salon_night_rain",
    "prairie-queen/inside_night_snow.png": "pq_salon_night_winter",
    "prairie-queen/outside_day.png": "pq_landing_day",
    "prairie-queen/outside_day_rain.png": "pq_landing_day_rain",
    "prairie-queen/outside_day_snow.png": "pq_landing_day_winter",
    "prairie-queen/outside_deck_night.png": "pq_deck_night",
    "prairie-queen/outside_night.png": "pq_landing_night",
    "prairie-queen/outside_rain.png": "pq_landing_night_rain",
    "prairie-queen/outside_snow.png": "pq_landing_night_winter",
    "slackwater_motel/inside_office_day.png": "motel_office_day",
    "slackwater_motel/inside_office_day_rain.png": "motel_office_day_rain",
    "slackwater_motel/inside_office_day_snow.png": "motel_office_day_winter",
    "slackwater_motel/inside_office_night.png": "motel_office_night",
    "slackwater_motel/inside_office_night_rain.png": "motel_office_night_rain",
    "slackwater_motel/inside_office_night_snow.png": "motel_office_night_winter",
    "slackwater_motel/outside_day.png": "motel_lot_day",
    "slackwater_motel/outside_day_rain.png": "motel_lot_day_rain",
    "slackwater_motel/outside_day_winter.png": "motel_lot_day_winter",
    "slackwater_motel/outside_night.png": "motel_lot_night",
    "slackwater_motel/outside_night_rain.png": "motel_lot_night_rain",
    "slackwater_motel/outside_night_winter.png": "motel_lot_night_winter",
    "slackwater_motel/room_day.png": "motel_room_day",
    "slackwater_motel/room_day_rain.png": "motel_room_day_rain",
    "slackwater_motel/room_day_snow.png": "motel_room_day_winter",
    "slackwater_motel/room_night.png": "motel_room_night",
    "slackwater_motel/room_night_rain.png": "motel_room_night_rain",
    "slackwater_motel/room_night_snow.png": "motel_room_night_winter",
}


def folders(style):
    resources = ROOT / "game/Assets/Resources"
    if style == "base":
        return ROOT / "Images/Locations", resources / "Plates"
    return ROOT / f"Images{style.capitalize()}/Locations", resources / f"Styles/{style}/Plates"


def main():
    parser = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    parser.add_argument("--style", default="base", help="art style: base (Images/) or e.g. alt (ImagesAlt/)")
    parser.add_argument("--dry-run", action="store_true", help="say what would change without copying")
    args = parser.parse_args()

    source, target = folders(args.style)
    if not source.is_dir():
        raise SystemExit(f"No drop folder at {source.relative_to(ROOT)}")

    copied = unchanged = 0
    unmapped = []
    for path in sorted(source.rglob("*.png")):
        key = path.relative_to(source).as_posix()
        tag = SOURCES.get(key)
        if tag is None:
            unmapped.append(key)
            continue
        dest = target / f"{tag}.png"
        if dest.exists() and filecmp.cmp(path, dest, shallow=False):
            unchanged += 1
            continue
        print(f"{'would copy' if args.dry_run else 'copied'}  {key}  ->  {dest.relative_to(ROOT)}")
        if not args.dry_run:
            dest.parent.mkdir(parents=True, exist_ok=True)
            shutil.copyfile(path, dest)
        copied += 1

    print(f"{copied} {'to copy' if args.dry_run else 'copied'}, {unchanged} already up to date.")
    if unmapped:
        print("No plate tag in SOURCES for (add one to bring it in):")
        for key in unmapped:
            print(f"  Locations/{key}")


if __name__ == "__main__":
    main()
