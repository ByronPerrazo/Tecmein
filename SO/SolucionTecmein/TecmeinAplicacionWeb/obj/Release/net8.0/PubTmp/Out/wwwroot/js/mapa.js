
function iniciarMap() {
    
    var coord = { lat: -1.3144194, lng: -94.6194873 };
    var map = new google.maps.Map(document.getElementById('map'), {
        zoom: 10,
        center: coord
    });
    var marker = new google.maps.Marker({
        position: coord,
        map: map
    });
}