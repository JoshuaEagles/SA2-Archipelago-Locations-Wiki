"use strict";

setupLocationGuideScript();

function setupLocationGuideScript() {
	if (!window.location.href.includes("Locations")) {
		return;
	}

	// Add the toggle buttons to the page in js
	// Done this way so that if you load the page without js enabled it won't show pointless buttons
	addToggleButtonsHtml();

	setupLocationTypeToggling();
	setupChronologicalToggling();
}

function setupLocationTypeToggling() {
	const locationTypeNames = ["chaobox", "pipe", "hidden", "goldbeetle", "omochao", "animal", "item", "life", "big"];

	locationTypeNames.forEach(function (locationTypeName) {
		setupLocationTypeFilterButtonListener(locationTypeName);
	});

	function setupLocationTypeFilterButtonListener(locationTypeName) {
		const locationTypeCheckbox = document.querySelector(`#${locationTypeName}-toggle`);

		locationTypeCheckbox.addEventListener("click", function () {
			handleFilterButtonClick();
		});
		locationTypeCheckbox.addEventListener("touchend", function () {
			handleFilterButtonClick();
		});

		function handleFilterButtonClick() {
			locationTypeCheckbox.dataset.toggled = !(locationTypeCheckbox.dataset.toggled === "true");
			locationTypeCheckbox.innerHTML = locationTypeCheckbox.dataset.toggled === "true" ? "Enabled" : "Disabled";

			const locationElementsOfType = document.querySelectorAll(`.location[data-type=${locationTypeName}]`);
			locationElementsOfType.forEach(function (location) {
				location.classList.toggle("disabled", !(locationTypeCheckbox.dataset.toggled === "true"));
			});

			// Handle special case of big locations manually, if more types start differing between logic difficulties then it'd be worth handling it more gracefully
			if (locationTypeName === "big") {
				const locationElementsForBigNormal = document.querySelectorAll(`.location[data-type=bignormal]`);
				locationElementsForBigNormal.forEach(function (location) {
					location.classList.toggle("disabled", !(locationTypeCheckbox.dataset.toggled === "true"));
				});

				const locationElementsForBigHard = document.querySelectorAll(`.location[data-type=bighard]`);
				locationElementsForBigHard.forEach(function (location) {
					location.classList.toggle("disabled", !(locationTypeCheckbox.dataset.toggled === "true"));
				});
			}
		}
	}
}

function setupChronologicalToggling() {
	const locationWrapper = document.querySelector("#location-wrapper");

	const initialLocationWrapperInnerHtml = locationWrapper.innerHTML;
	const chronologicalOrderingDocumentFragment = generateChronologicalDocumentFragment();

	let isChronologicalEnabled = false;

	const toggleChronologicalCheckbox = document.querySelector("#chronological-toggle");
	toggleChronologicalCheckbox.addEventListener("click", function () {
		handleChronologicalButtonClick();
	});
	toggleChronologicalCheckbox.addEventListener("touchend", function () {
		handleChronologicalButtonClick();
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

	function handleChronologicalButtonClick() {
		toggleChronologicalCheckbox.dataset.toggled = !(toggleChronologicalCheckbox.dataset.toggled === "true");
		toggleChronologicalCheckbox.innerHTML =
			toggleChronologicalCheckbox.dataset.toggled === "true" ? "Enabled" : "Disabled";

		if (toggleChronologicalCheckbox.dataset.toggled === "true") {
			changeToDisplayByChronological();
		} else {
			changeToDisplayByType();
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

function addToggleButtonsHtml() {
	let toggleButtonsElementHtml = `
			<div>
				<span class="toggle-label" style="margin-right">Display Chaobox Locations: </span>
				<span class="toggle-button" id="chaobox-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Pipe Locations: </span>
				<span class="toggle-button" id="pipe-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Hidden Locations: </span>
				<span class="toggle-button" id="hidden-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Gold Beetle Locations: </span>
				<span class="toggle-button" id="goldbeetle-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Omochao Locations: </span>
				<span class="toggle-button" id="omochao-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Animal Locations: </span>
				<span class="toggle-button" id="animal-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Item Locations: </span>
				<span class="toggle-button" id="item-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Life Locations: </span>
				<span class="toggle-button" id="life-toggle" data-toggled="true">Enabled</span>
			</div>

			<div>
				<span class="toggle-label" style="margin-right">Display Big Locations: </span>
				<span class="toggle-button" id="big-toggle" data-toggled="true">Enabled</span>
			</div>

			<hr />

			<div>
				<span class="toggle-label" style="margin-right">Display in Chronological Order: </span>
				<span class="toggle-button" id="chronological-toggle" data-toggled="false">Disabled</span>
			</div>
		`;

	let togglesContainerElement = document.getElementById("toggles-container");
	togglesContainerElement.innerHTML = toggleButtonsElementHtml;
}
