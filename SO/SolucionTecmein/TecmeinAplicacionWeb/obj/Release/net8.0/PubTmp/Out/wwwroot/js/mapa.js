
(g => { var h, a, k, p = "The Google Maps JavaScript API", c = "google", l = "importLibrary", q = "__ib__", m = document, b = window; b = b[c] || (b[c] = {}); var d = b.maps || (b.maps = {}), r = new Set, e = new URLSearchParams, u = () => h || (h = new Promise(async (f, n) => { await (a = m.createElement("script")); e.set("libraries", [...r] + ""); for (k in g) e.set(k.replace(/[A-Z]/g, t => "_" + t[0].toLowerCase()), g[k]); e.set("callback", c + ".maps." + q); a.src = `https://maps.${c}apis.com/maps/api/js?` + e; d[q] = f; a.onerror = () => h = n(Error(p + " could not load.")); a.nonce = m.querySelector("script[nonce]")?.nonce || ""; m.head.append(a) })); d[l] ? console.warn(p + " only loads once. Ignoring:", g) : d[l] = (f, ...n) => r.add(f) && u().then(() => d[l](f, ...n)) })
    ({ key: "AIzaSyCJ2mSr_Ve_GjsUmJxulhAvuuk6Sr6lh3Y", v: "weekly" });


// Initialize and add the map
let map;

async function initMap(coordenadasString) {
    // The location of Uluru
    //-0.11922587199662392,-78.4688364392997
    //const position = { lat: -0.1192258, lng: -78.4688 };
    // Request needed libraries.
    //@ts-ignore

    let coordenadasArray = coordenadasString.split(",");
    let latitudString = coordenadasArray[0];
    let longitudString = coordenadasArray[1];

    // Convertir las cadenas a números utilizando parseInt
    let latitud = parseFloat(latitudString);
    let longitud = parseFloat(longitudString);

    // Crear la posición con las coordenadas convertidas
    const position = { lat: latitud, lng: longitud };

    const { Map } = await google.maps.importLibrary("maps");
    const { AdvancedMarkerElement } = await google.maps.importLibrary("marker");

    // The map, centered at Uluru
    map = new Map(document.getElementById("map"), {
        zoom: 15,
        center: position,
        mapId: "DEMO_MAP_ID",
    });

    // The marker, positioned at Uluru
    const marker = new AdvancedMarkerElement({
        map: map,
        position: position,
        title: "Uluru",
    });
}

initMap("-0.11922587199662392,-78.4688364392997");
