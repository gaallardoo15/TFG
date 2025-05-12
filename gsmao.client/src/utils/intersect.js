export function intersect(a, b) {
    // If either a or b is empty or not an array, return an empty array
    if (!Array.isArray(a) || !Array.isArray(b) || !a.length || !b.length) {
        return [];
    }
    var t;
    if (b.length > a.length) {
        (t = b), (b = a), (a = t);
    } // indexOf to loop over shorter
    return a
        .filter(function (e) {
            return b.indexOf(e) > -1;
        })
        .filter(function (e, i, c) {
            // extra step to remove duplicates
            return c.indexOf(e) === i;
        });
}
 
 
