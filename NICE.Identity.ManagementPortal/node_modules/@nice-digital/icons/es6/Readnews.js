import React from "react";

const SvgReadnews = props => (
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
			d="M459.196 235.576H430.78v-102.4l-175.04 115.76-175.024-115.76v102.4h-28.4c-23.088 0-23.088 35.76 0 35.76h28.4v93.36l175.024 95.296 175.04-95.296v-93.36h28.416c23.024 0 23.024-35.76 0-35.76zm-137.52-116.752c0 36.352-29.504 65.824-65.936 65.824s-65.936-29.472-65.936-65.824S219.308 53 255.74 53s65.936 29.472 65.936 65.824z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgReadnews;
