namespace KingdomBoard.Structure {

    using Enum;
    using Helpers;
    using Scriptable;

    public interface IStructure : IHasHealth, ISelected {

        StructureScriptable Data { get; }
        StructureClassType classType { get; }
        StructureType structureType { get; }
    } 
}