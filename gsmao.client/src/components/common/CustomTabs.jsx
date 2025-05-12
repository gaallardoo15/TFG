import {Children} from "react";
import {Tab, Tabs} from "react-bootstrap";

export const CustomTabs = ({defaultActiveKey, children, onSelect, ...rest}) => {
    return (
        <Tabs defaultActiveKey={defaultActiveKey} className="tabsPersonalizados" {...rest} fill onSelect={onSelect}>
            {Children.map(children, (child) => (
                <Tab eventKey={child.props.eventKey} className="p-3" title={child.props.title}>
                    {child}
                </Tab>
            ))}
        </Tabs>
    );
};
