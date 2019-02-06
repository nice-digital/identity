import React from "react";

const SvgDownload = props => (
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
			d="M35 392.067h443V477H35v-84.933zm295.387-169.914V34H182.693v188.153h-73.822l147.918 169.93 147.372-169.93h-73.774z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgDownload;
