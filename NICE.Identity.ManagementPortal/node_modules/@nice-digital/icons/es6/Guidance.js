import React from "react";

const SvgGuidance = props => (
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
			d="M361.488 476.472l-105.744-96.944L150 476.472V35h211.568v441.472h-.08z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgGuidance;
