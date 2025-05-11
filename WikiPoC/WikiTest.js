"use strict";

const locationTypeNamesInOrder = [
	"chaobox",
	"pipe",
	"hidden",
	"goldbeetle",
	"omochao",
	"animal",
	"item",
	"life",
	"big",
];

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
	locationTypeNamesInOrder.forEach((locationTypeName) => {
		setupLocationTypeFilterButtonListener(locationTypeName);
	});
	setupToggleAllFiltersButtonListener();

	function setupLocationTypeFilterButtonListener(locationTypeName) {
		const locationTypeFilterButton = document.querySelector(`#${locationTypeName}-toggle`);

		locationTypeFilterButton.addEventListener("click", () => {
			handleFilterButtonClick();
		});

		function handleFilterButtonClick() {
			locationTypeFilterButton.dataset.toggled = !(locationTypeFilterButton.dataset.toggled === "true");
			locationTypeFilterButton.innerHTML = locationTypeFilterButton.dataset.toggled === "true" ? "Enabled" : "Disabled";

			const locationElementsOfType = document.querySelectorAll(`.location[data-type=${locationTypeName}]`);
			locationElementsOfType.forEach((location) => {
				location.classList.toggle("disabled", !(locationTypeFilterButton.dataset.toggled === "true"));
			});

			// Handle special case of big locations manually, if more types start differing between logic difficulties then it'd be worth handling it more gracefully
			if (locationTypeName === "big") {
				const locationElementsForBigNormal = document.querySelectorAll(`.location[data-type=bignormal]`);
				locationElementsForBigNormal.forEach((location) => {
					location.classList.toggle("disabled", !(locationTypeFilterButton.dataset.toggled === "true"));
				});

				const locationElementsForBigHard = document.querySelectorAll(`.location[data-type=bighard]`);
				locationElementsForBigHard.forEach((location) => {
					location.classList.toggle("disabled", !(locationTypeFilterButton.dataset.toggled === "true"));
				});
			}
		}
	}

	function setupToggleAllFiltersButtonListener() {
		const locationTypeFilterButton = document.querySelector(`#all-filters-toggle`);
		locationTypeFilterButton.addEventListener("click", () => handleToggleAllFiltersButtonClick());

		function handleToggleAllFiltersButtonClick() {
			const allToggleButtons = Array.from(document.querySelectorAll(".toggle-button:not(#chronological-toggle)"));
			const allDisabledToggleButtons = allToggleButtons.filter((button) => button.dataset.toggled == "false");

			// If any are disabled, click those to enable them
			if (allDisabledToggleButtons.length > 0) {
				allDisabledToggleButtons.forEach((button) => button.click());
				return;
			}

			// If all are enabled, click them all to disable them
			allToggleButtons.forEach((button) => button.click());
		}
	}
}

function setupChronologicalToggling() {
	const locationWrapper = document.querySelector("#location-wrapper");

	const toggleChronologicalCheckbox = document.querySelector("#chronological-toggle");
	toggleChronologicalCheckbox.addEventListener("click", () => {
		handleChronologicalButtonClick();
	});

	function handleChronologicalButtonClick() {
		toggleChronologicalCheckbox.dataset.toggled = !(toggleChronologicalCheckbox.dataset.toggled === "true");
		toggleChronologicalCheckbox.innerHTML =
			toggleChronologicalCheckbox.dataset.toggled === "true" ? "Enabled" : "Disabled";

		if (toggleChronologicalCheckbox.dataset.toggled === "true") {
			sortPageIntoChronologicalOrder();
			toggleTypeHeaderAndTableOfContents(false);
		} else {
			sortPageIntoByTypeOrder();
			toggleTypeHeaderAndTableOfContents(true);
		}
	}

	function sortPageIntoChronologicalOrder() {
		const initialLocationWrapperInnerHtml = locationWrapper.innerHTML;

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
				const locationAlreadyHandledErrorMessage =
					"Error encountered while generating the chronological page. A location Id that was already processed was processed again. This would lead to a loop.";
				console.error(locationAlreadyHandledErrorMessage);

				// This will make the chronological button do nothing since this is what the page would display without clicking that button
				return initialLocationWrapperInnerHtml;
			}

			const nextLocationElement = locationIdToLocationElementMap.get(nextLocationId);
			if (!nextLocationElement) {
				const chronologicalNextLocationDoesNotExistErrorMessage = `Error encountered while generating the chronological page. The nextChronologicalLocation "${nextLocationId}" referred to a location that does not exist on this page.`;
				console.error(chronologicalNextLocationDoesNotExistErrorMessage);

				// This will make the chronological button do nothing since this is what the page would display without clicking that button
				return initialLocationWrapperInnerHtml;
			}

			sortedLocationElementsDocumentFragment.appendChild(nextLocationElement);

			nextLocationId = nextLocationElement.dataset.chronologicalNextLocation;
		}

		locationWrapper.innerHTML = null;
		locationWrapper.appendChild(sortedLocationElementsDocumentFragment);

		function setupMappingOfLocationIdToLocationElement() {
			const locationMap = new Map();
			allLocations.forEach((location) => {
				locationMap.set(location.id, location);
			});

			return locationMap;
		}
	}

	function sortPageIntoByTypeOrder() {
		const sortedLocationElementsDocumentFragment = document.createDocumentFragment();

		locationTypeNamesInOrder.forEach((typeName) => {
			const locationsOfTypeNodeList = document.querySelectorAll(`.location[data-type=${typeName}]`);
			const locationsOfTypeArray = Array.from(locationsOfTypeNodeList);

			locationsOfTypeArray.sort((a, b) => {
				// This handles sorting big properly, bignormal and bighard sort in the reverse order we want,
				// but big1 and big2 would sort in the right order
				const aId = a.id.replace("bignormal", "big1").replace("bighard", "big2");
				const bId = b.id.replace("bignormal", "big1").replace("bighard", "big2");

				if (aId < bId) {
					return -1;
				}
				if (aId > bId) {
					return 1;
				}

				return 0;
			});
			locationsOfTypeArray.forEach((location) => {
				sortedLocationElementsDocumentFragment.appendChild(location);
			});
		});

		locationWrapper.innerHtml = null;
		locationWrapper.appendChild(sortedLocationElementsDocumentFragment);
	}

	function toggleTypeHeaderAndTableOfContents(isShown) {
		const tableOfContentsContainer = document.querySelector(`#location-wiki-toc`);
		tableOfContentsContainer.classList.toggle("disabled", !isShown);

		const locationTypeHeaders = document.querySelectorAll(`#location-wrapper .location-type-header`);
		locationTypeHeaders.forEach((header) => {
			header.classList.toggle("disabled", !isShown);
		});
	}
}

function addToggleButtonsHtml() {
	let toggleButtonsElementHtml = `
			<div class="single-toggle-container">
				<span class="toggle-label">Display in Chronological Order: </span>
				<span class="button toggle-button" id="chronological-toggle" data-toggled="false">Disabled</span>
			</div>

			<hr />

			<div class="single-toggle-container">
				<span class="toggle-label">Display Chaobox Locations: </span>
				<span class="button toggle-button" id="chaobox-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Pipe Locations: </span>
				<span class="button toggle-button" id="pipe-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Hidden Locations: </span>
				<span class="button toggle-button" id="hidden-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Gold Beetle Locations: </span>
				<span class="button toggle-button" id="goldbeetle-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Omochao Locations: </span>
				<span class="button toggle-button" id="omochao-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Animal Locations: </span>
				<span class="button toggle-button" id="animal-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Item box Locations: </span>
				<span class="button toggle-button" id="item-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Life box Locations: </span>
				<span class="button toggle-button" id="life-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="single-toggle-container">
				<span class="toggle-label">Display Big Locations: </span>
				<span class="button toggle-button" id="big-toggle" data-toggled="true">Enabled</span>
			</div>

			<div class="toggle-all-filters-container">
				<span class="button" id="all-filters-toggle" data-toggled="true">Toggle all Filters</span>
			</div>
		`;

	let togglesContainerElement = document.getElementById("toggles-container");
	togglesContainerElement.innerHTML = toggleButtonsElementHtml;
}
