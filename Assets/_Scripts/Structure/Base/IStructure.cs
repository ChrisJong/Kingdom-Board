namespace Structure {

    using Enum;
    using Helpers;

    public interface IStructure : IHasHealth, ISelected {

        StructureType structureType { get; }

        bool isReady { get; set; } 
    } 
}