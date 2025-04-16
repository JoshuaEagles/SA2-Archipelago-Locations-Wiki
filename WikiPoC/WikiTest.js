// Setup pipe toggling
let pipeLocations = document.querySelectorAll(".location[data-type=pipe]");
let hiddenLocations = document.querySelectorAll(".location[data-type=hidden]");
let allLocations = document.querySelectorAll(".location");

let pipeToggleCheckbox = document.querySelector("#pipe-toggle");
pipeToggleCheckbox.addEventListener("change", function () {
	let checked = this.checked;
	pipeLocations.forEach(function (pipeLocation) {
		pipeLocation.classList.toggle("disabled", !checked);
	});
});

let hiddenToggleCheckbox = document.querySelector("#hidden-toggle");
hiddenToggleCheckbox.addEventListener("change", function () {
	let checked = this.checked;
	hiddenLocations.forEach(function (hiddenLocation) {
		hiddenLocation.classList.toggle("disabled", !checked);
	});
});

let chronologicalState = false;
let toggleChronologicalButton = document.querySelector("#chronological-toggle");
let typeOrdering = ["pipe", "hidden"];
let locationMap = new Map();
toggleChronologicalButton.addEventListener("click", function () {
	chronologicalState = !chronologicalState;

	if (chronologicalState == true) {
		// Chronological enabled
		allLocations.forEach(function (location) {
			locationMap.set(location.id, location);
		});
		console.log(locationMap);

		let firstChronologicalLocation = document.querySelector(
			"#chronological-first-location",
		).dataset.chronologicalFirstLocation;
		let nextLocationId = firstChronologicalLocation;

		let sortedLocations = document.createDocumentFragment();
		while (nextLocationId) {
			let nextLocation = locationMap.get(nextLocationId);
			sortedLocations.appendChild(nextLocation);
			nextLocationId = nextLocation.dataset.chronologicalNextLocation;
		}

		let locationWrapper = document.querySelector("#location-wrapper");
		locationWrapper.innerHtml = null;
		locationWrapper.appendChild(sortedLocations);
	} else {
		let sortedLocations = document.createDocumentFragment();

		typeOrdering.forEach(function (typeName) {
			let locationsOfTypeNodeList = document.querySelectorAll(
				`.location[data-type=${typeName}]`,
			);
			let locationsOfTypeArray = Array.prototype.slice.call(
				locationsOfTypeNodeList,
				0,
			);
			locationsOfTypeArray.sort(function (a, b) {
				if (a.id < b.id) {
					return -1;
				}
				if (a.id > b.id) {
					return 1;
				}

				return 0;
			});
			locationsOfTypeArray.forEach(function (location) {
				sortedLocations.appendChild(location);
			});
		});

		let locationWrapper = document.querySelector("#location-wrapper");
		locationWrapper.innerHtml = null;
		locationWrapper.appendChild(sortedLocations);
	}
});
// type: location.dataset.type,
// nextLocation: location.dataset.chronologicalNextLocation,
