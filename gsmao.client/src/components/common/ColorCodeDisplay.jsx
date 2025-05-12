import {getFontColor, getHexColor} from "@/utils/itemColor";

export const ColorCodeDisplay = ({colorCode}) => {
    return (
        <code
            style={{
                backgroundColor: getHexColor(colorCode),
                color: getFontColor(colorCode),
                borderRadius: "5px",
                padding: "1px 4px",
                border: `1px solid black`,
            }}>
            {colorCode}
        </code>
    );
};
