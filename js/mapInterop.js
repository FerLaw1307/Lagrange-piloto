window.mapInterop = {
    map: null,
    markerLayer: null,

    initializeMap: function (element, markers) {
        if (!element) return;

        this.map = L.map(element).setView([10.0, -84.0], 6);

        L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
            attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors'
        }).addTo(this.map);

        this.markerLayer = L.layerGroup().addTo(this.map);
        this._updateMarkers(markers);
    },

    updateMarkers: function (markers) {
        if (!this.map) {
            console.warn('Leaflet map is not initialized yet.');
            return;
        }
        this._updateMarkers(markers);
    },

    _updateMarkers: function (markers) {
        if (!this.markerLayer) {
            this.markerLayer = L.layerGroup().addTo(this.map);
        }

        this.markerLayer.clearLayers();

        if (!markers || !Array.isArray(markers) || markers.length === 0) {
            return;
        }

        var bounds = [];
        markers.forEach(function (marker) {
            if (typeof marker.latitude !== 'number' || typeof marker.longitude !== 'number') {
                return;
            }

            var icon = L.icon({
                iconUrl: marker.type === 'corral'
                    ? 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png'
                    : 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-icon.png',
                iconSize: [25, 41],
                iconAnchor: [12, 41],
                popupAnchor: [1, -34],
                shadowUrl: 'https://unpkg.com/leaflet@1.9.4/dist/images/marker-shadow.png',
                shadowSize: [41, 41]
            });

            var popupText = '<strong>' + marker.title + '</strong><br/>' + marker.subtitle;
            var leafletMarker = L.marker([marker.latitude, marker.longitude], { icon: icon }).bindPopup(popupText);
            leafletMarker.addTo(this.markerLayer);
            bounds.push([marker.latitude, marker.longitude]);
        }, this);

        if (bounds.length > 0) {
            this.map.fitBounds(bounds, { padding: [40, 40] });
        }
    }
};
