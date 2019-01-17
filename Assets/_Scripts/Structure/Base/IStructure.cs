namespace Structure {

    using Enum;
    using Helpers;
    using Scriptable;

    public interface IStructure : IHasHealth, ISelected {

        StructureScriptable StructureData { get; set; }
        StructureType structureType { get; }
    } 
}