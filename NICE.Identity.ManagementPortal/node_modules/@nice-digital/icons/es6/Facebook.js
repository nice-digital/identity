import React from "react";

const SvgFacebook = props => (
	<svg
		width="1em"
		height="1em"
		viewBox="0 0 512 512"
		fill="none"
		className="icon"
		aria-hidden={true}
		{...props}
	>
		<path
			d="M291.069 178.811H361l-8.168 77.197h-61.778V480h-92.789V256.008H152v-77.197h46.265V132.32c0-32.983 7.802-57.95 23.407-74.897C237.276 40.474 262.949 32 298.689 32h61.778v77.196h-38.646c-7.071 0-12.74.589-17.006 1.766-4.267 1.177-7.351 3.309-9.25 6.394-1.9 3.085-3.124 6.211-3.673 9.377-.549 3.167-.823 7.652-.823 13.457v38.621z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgFacebook;
