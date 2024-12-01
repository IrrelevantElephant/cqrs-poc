interface FilterButtonProps {
  name: string;
  isPressed: boolean;
  setFilter: Function;
}

const FilterButton = ({ name, isPressed, setFilter }: FilterButtonProps) => {
  return (
    <button
      type="button"
      className="btn toggle-btn"
      aria-pressed={isPressed}
      onClick={() => setFilter(name)}
    >
      <span className="visually-hidden">Show </span>
      <span>{name}</span>
      <span className="visually-hidden"> tasks</span>
    </button>
  );
};

export default FilterButton;
