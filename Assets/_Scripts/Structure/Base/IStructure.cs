namespace Structure {

    using Enum;
    using Helpers;

    public interface IStructure : IHasHealth {

        StructureType structureType { get; }

        bool isReady { get; set; } 
    } 
}