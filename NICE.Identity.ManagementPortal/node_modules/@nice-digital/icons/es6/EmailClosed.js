import React from "react";

const SvgEmailClosed = props => (
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
			d="M35 93v326.624h441.472V93H35zm391.456 37.456l-170.784 130.64L85 130.456h341.456zm-354 37.568l91.504 70.048-91.504 91.504V168.008v.016zm.384 214.144l121.136-121.12 61.696 47.232 61.408-46.976 120.864 120.864H72.84zm366.176-51.92l-91.92-91.92 91.92-70.32v162.24z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgEmailClosed;
