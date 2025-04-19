const locationTypeNames = ["pipe", "hidden"];

const locationWrapper = document.querySelector("#location-wrapper");

setupLocationTypeToggling();
setupChronologicalToggling();

function setupLocationTypeToggling() {
	locationTypeNames.forEach(function (locationTypeName) {
		setupLocationTypeFilterButtonListener(locationTypeName);
	});

	function setupLocationTypeFilterButtonListener(locationTypeName) {
		let locationTypeCheckbox = document.querySelector(
			`#${locationTypeName}-toggle`,
		);

		let locationElementsOfType = document.querySelectorAll(
			`.location[data-type=${locationTypeName}]`,
		);

		locationTypeCheckbox.addEventListener("change", function () {
			let checked = this.checked;
			locationElementsOfType.forEach(function (location) {
				location.classList.toggle("disabled", !checked);
			});
		});
	}
}

function setupChronologicalToggling() {
	let toggleChronologicalButton = document.querySelector(
		"#chronological-toggle",
	);
	let chronologicalState = false;
	toggleChronologicalButton.addEventListener("click", function () {
		chronologicalState = !chronologicalState;

		if (chronologicalState === true) {
			sortLocationsChronologically();
		} else {
			sortLocationsByType();
		}
	});

	function sortLocationsChronologically() {
		const allLocations = document.querySelectorAll(".location");

		let locationMap = new Map();

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

		locationWrapper.innerHtml = null;
		locationWrapper.appendChild(sortedLocations);
	}

	function sortLocationsByType() {
		const sortByTypeOrdering = ["pipe", "hidden"];

		let sortedLocations = document.createDocumentFragment();

		sortByTypeOrdering.forEach(function (typeName) {
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

		locationWrapper.innerHtml = null;
		locationWrapper.appendChild(sortedLocations);
	}
}

// type: location.dataset.type,
// nextLocation: location.dataset.chronologicalNextLocation,

// TODO: We'll need a mapping of location types to title text
// TODO: The location type ordering array will need to be expanded
// TODO: The code needs to be made generic, only data should need to be edited to support more location types
//

// Idea: can we store the state on page load and consider that the sort by type, and then after we do the chronological sort once cache the edited sort?
// That would simplify the code and make it faster
