using Imagin.Core.Collections.Generic;
using System;

namespace Imagin.Core.Collections.Serialization
{
    public class GroupWriter<T> : BinaryWriter<GroupCollection<T>>, IGroupWriter
    {
        public GroupWriter(string folderPath, string fileName, string fileExtension, string singleFileExtension, Limit limit) : base(folderPath, fileName, fileExtension, singleFileExtension, limit) { }

        public Type GetItemType() => typeof(GroupCollection<T>);
    }
}