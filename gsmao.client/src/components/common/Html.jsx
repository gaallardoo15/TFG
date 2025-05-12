export const Html = ({children, element: Element = "div"}) => (
    <Element
        dangerouslySetInnerHTML={{
            __html: children,
        }}
    />
);

// Html.defaultProps = {
//     element: "div",
// };
