"use strict";

setupLocationGuideScript();

function setupLocationGuideScript() {
	if (!window.location.href.includes("Locations")) {
		return;
	}

	setupLocationTypeToggling();
	setupChronologicalToggling();
}

function setupLocationTypeToggling() {
	const locationTypeNames = ["pipe", "hidden"];

	locationTypeNames.forEach(function (locationTypeName) {
		setupLocationTypeFilterButtonListener(locationTypeName);
	});

	function setupLocationTypeFilterButtonListener(locationTypeName) {
		const locationTypeCheckbox = document.querySelector(`#${locationTypeName}-toggle`);

		const locationElementsOfType = document.querySelectorAll(`.location[data-type=${locationTypeName}]`);

		locationTypeCheckbox.addEventListener("change", function () {
			const isChecked = this.checked;

			locationElementsOfType.forEach(function (location) {
				location.classList.toggle("disabled", !isChecked);
			});
		});
	}
}

function setupChronologicalToggling() {
	const locationWrapper = document.querySelector("#location-wrapper");

	const initialLocationWrapperInnerHtml = locationWrapper.innerHTML;
	const chronologicalOrderingDocumentFragment = generateChronologicalDocumentFragment();

	let isChronologicalEnabled = false;

	const toggleChronologicalCheckbox = document.querySelector("#chronological-toggle");
	toggleChronologicalCheckbox.addEventListener("click", function () {
		isChronologicalEnabled = !isChronologicalEnabled;

		if (isChronologicalEnabled === true) {
			changeToDisplayByChronological();
		} else {
			changeToDisplayByType();
		}
	});

	function generateChronologicalDocumentFragment() {
		const allLocations = document.querySelectorAll(".location");

		const locationIdToLocationElementMap = setupMappingOfLocationIdToLocationElement();

		// We have a special element that sets the starting point, then each element will point to the next
		// This is effectively a linked list defined through html elements
		const firstChronologicalLocationId = document.querySelector("#chronological-first-location").dataset
			.chronologicalFirstLocation;

		const sortedLocationElementsDocumentFragment = document.createDocumentFragment();
		let nextLocationId = firstChronologicalLocationId;

		// We'll add every location we handle to here, if we try processing a location we already handled then that would mean the list loops back on itself
		// This will let us prevent recursion from being an issue
		const handledLocationIds = new Set();

		// When we get to an element that doesn't have a chronologicalNextLocation, this will be undefined, and therefore the loop will end
		while (nextLocationId) {
			if (handledLocationIds.has(nextLocationId)) {
				console.error(
					"Error encountered while generating the chronological page. a location Id that was already processed was processed again. This would lead to a loop.",
				);

				// This will make the chronological button do nothing since this is what the page would display without clicking that button
				return initialLocationWrapperInnerHtml;
			}

			const nextLocationElement = locationIdToLocationElementMap.get(nextLocationId);
			if (!nextLocationElement) {
				console.error(
					`Error encountered while generating the chronological page. The nextChronologicalLocation "${nextLocationId}" referred to a location that does not exist on this page.`,
				);

				// This will make the chronological button do nothing since this is what the page would display without clicking that button
				return initialLocationWrapperInnerHtml;
			}

			sortedLocationElementsDocumentFragment.appendChild(nextLocationElement.cloneNode(true));

			nextLocationId = nextLocationElement.dataset.chronologicalNextLocation;
		}

		return sortedLocationElementsDocumentFragment;

		function setupMappingOfLocationIdToLocationElement() {
			const locationMap = new Map();
			allLocations.forEach(function (location) {
				locationMap.set(location.id, location);
			});

			return locationMap;
		}
	}

	function changeToDisplayByChronological() {
		locationWrapper.innerHTML = null;
		locationWrapper.appendChild(chronologicalOrderingDocumentFragment.cloneNode(true));
	}

	function changeToDisplayByType() {
		locationWrapper.innerHTML = initialLocationWrapperInnerHtml;
	}
}
