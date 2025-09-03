import {CheckIcon} from "@heroicons/react/24/solid";

type Props = {
    checked: boolean;
    onChange: (checked: boolean) => void;
    label?: string;
};
export const Checkbox = ({checked, onChange, label}: Props) => {
    return (
        <label className="container">
            <input
                type="checkbox"
                checked={checked}
                onChange={(e) => onChange(e.target.checked)}
            />
            <span className="checkmark-container">
                <CheckIcon className="checkmark"></CheckIcon>
            </span>
            <div className="label">
                {label}
            </div>
        </label>
    );
}