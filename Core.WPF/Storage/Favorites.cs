using Imagin.Core.Collections;
using Imagin.Core.Collections.Serialization;
using Imagin.Core.Linq;
using System;
using System.Linq;

namespace Imagin.Core.Storage
{
    [Serializable]
    public class Favorites : XmlWriter<Favorite>
    {
        public void Is(bool @is, string folderPath)
        {
            var found = this.FirstOrDefault(i => i.Path == folderPath);
            if (!@is)
            {
                found.If(i => i != null, i => Remove(i));
            }
            else if (found == null)
                Add(new Favorite(folderPath));
        }

        public Favorites(string folderPath, Limit limit = default) : base(nameof(Favorites), folderPath, nameof(Favorites), "xml", "favorite", limit, typeof(Favorite)) { }
    }
}