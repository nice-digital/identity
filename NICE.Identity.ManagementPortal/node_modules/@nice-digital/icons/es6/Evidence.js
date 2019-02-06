import React from "react";

const SvgEvidence = props => (
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
			d="M255.912 39C136.12 39 39 136.312 39 256.328s97.12 217.344 216.912 217.344c119.76 0 216.864-97.312 216.864-217.344S375.672 39 255.912 39zm-.016 314.464c-53.536 0-96.928-43.488-96.928-97.136 0-53.648 43.392-97.12 96.928-97.12 53.536 0 96.928 43.488 96.928 97.12 0 53.632-43.392 97.136-96.928 97.136z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgEvidence;
