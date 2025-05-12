const SearchTableInput = ({searchTerm, setSearchTerm}) => {
    return (
        <input
            type="text"
            placeholder="Buscar..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="form-control mb-3"
        />
    );
};

export default SearchTableInput;
