using System;
using System.Collections.Generic;
using UnityEngine;

namespace Starflare.Game
{
    /// <summary>
    /// One crowd face in the roster (Images/Avatars/avatars.json, copied to
    /// Resources/Faces/faces.json by tools/import_faces.py): a walk-on shown as a face only.
    /// Fields are lower case to match the roster's JSON, which <see cref="JsonUtility"/> reads by name.
    /// </summary>
    [Serializable]
    public sealed class CrowdFace
    {
        public string id;
        public string name;

        /// <summary>"patron", or later "worker" for venue staff.</summary>
        public string type;

        /// <summary>The place ids (story.js) where this face may turn up.</summary>
        public string[] venues;

        /// <summary>What the circle calls them: "Marcus", not "Marcus Bell".</summary>
        public string FirstName
        {
            get
            {
                if (string.IsNullOrEmpty(name)) return "";
                int space = name.IndexOf(' ');
                return space < 0 ? name : name.Substring(0, space);
            }
        }

        public bool IsPatron => type == "patron";

        public bool TurnsUpAt(string placeId) => venues != null && Array.IndexOf(venues, placeId) >= 0;
    }

    /// <summary>
    /// The crowd roster read from its JSON. The roster is an object with an "avatars" list,
    /// which is what lets <see cref="JsonUtility"/> read it (it can't read a bare list).
    /// </summary>
    public static class FaceRoster
    {
        [Serializable]
        sealed class File
        {
            public CrowdFace[] avatars;
        }

        public static CrowdFace[] Parse(string json)
        {
            if (string.IsNullOrEmpty(json)) return new CrowdFace[0];
            return JsonUtility.FromJson<File>(json)?.avatars ?? new CrowdFace[0];
        }

        /// <summary>The patrons who may turn up at this place, named ones only, in roster order.</summary>
        public static List<CrowdFace> PatronsAt(IEnumerable<CrowdFace> faces, string placeId)
        {
            var found = new List<CrowdFace>();
            foreach (var face in faces)
                if (face.IsPatron && !string.IsNullOrEmpty(face.name) && face.TurnsUpAt(placeId)) found.Add(face);
            return found;
        }
    }
}
