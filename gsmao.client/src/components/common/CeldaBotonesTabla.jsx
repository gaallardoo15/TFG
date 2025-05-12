export const CeldaBotonesTabla = ({availableActions, buttonByAction}) => (
    <div className="celdaBotonesTabla text-center">
        {availableActions
            ?.map((action) => buttonByAction[action])
            .reduce((accu, elem, index) => {
                return accu === null ? [elem] : [...accu, " ", elem];
            }, null)}
    </div>
);
