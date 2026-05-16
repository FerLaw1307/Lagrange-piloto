window.mapInterop = {
    map: null,
    markers: [],

    initializeMap: async function (element, apiKey, markers) {
        if (!element) return;

        await this._loadGoogleMapsAsync(apiKey);
        if (!window.google || !window.google.maps) {
            console.error('Google Maps failed to load.');
            return;
        }

        this.map = new google.maps.Map(element, {
            center: { lat: 10.0, lng: -84.0 },
            zoom: 6,
            mapTypeId: 'roadmap'
        });

        this._updateMarkers(markers);
    },

    updateMarkers: function (markers) {
        if (!this.map) {
            console.warn('Google Maps is not initialized yet.');
            return;
        }

        this._updateMarkers(markers);
    },

    _updateMarkers: function (markers) {
        if (!Array.isArray(markers)) {
            return;
        }

        this.markers.forEach(function (marker) {
            marker.setMap(null);
        });
        this.markers = [];

        if (markers.length === 0) {
            return;
        }

        var bounds = new google.maps.LatLngBounds();

        markers.forEach(function (markerData) {
            if (typeof markerData.latitude !== 'number' || typeof markerData.longitude !== 'number') {
                return;
            }

            var position = { lat: markerData.latitude, lng: markerData.longitude };
            var marker = new google.maps.Marker({
                position: position,
                map: this.map,
                title: markerData.title
            });

            var infoWindow = new google.maps.InfoWindow({
                content: '<div><strong>' + markerData.title + '</strong><br/>' + markerData.subtitle + '</div>'
            });

            marker.addListener('click', function () {
                infoWindow.open(this.map, marker);
            }.bind(this));

            this.markers.push(marker);
            bounds.extend(position);
        }, this);

        if (!bounds.isEmpty()) {
            this.map.fitBounds(bounds, { padding: 40 });
        }
    },

    _loadGoogleMapsAsync: function (apiKey) {
        return new Promise(function (resolve, reject) {
            if (window.google && window.google.maps) {
                resolve();
                return;
            }

            if (document.getElementById('google-maps-api')) {
                var checkLoaded = function () {
                    if (window.google && window.google.maps) {
                        resolve();
                    } else {
                        setTimeout(checkLoaded, 50);
                    }
                };
                checkLoaded();
                return;
            }

            var script = document.createElement('script');
            script.id = 'google-maps-api';
            script.src = 'https://maps.googleapis.com/maps/api/js?key=' + encodeURIComponent(apiKey);
            script.async = true;
            script.defer = true;
            script.onload = function () {
                resolve();
            };
            script.onerror = function (error) {
                reject(error);
            };
            document.head.appendChild(script);
        });
    }
};
