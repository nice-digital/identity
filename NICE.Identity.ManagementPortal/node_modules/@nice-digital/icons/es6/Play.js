import React from "react";

const SvgPlay = props => (
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
			d="M450.424 264.668L71 475.532c-4.384 2.475-8.144 2.763-11.28.864-3.136-1.898-4.71-5.328-4.72-10.288V45.532c0-4.949 1.573-8.378 4.72-10.288 3.147-1.909 6.907-1.621 11.28.864l379.424 210.864c4.384 2.475 6.576 5.43 6.576 8.864 0 3.435-2.192 6.39-6.576 8.864v-.032z"
			fill={props.colour || "currentColor"}
		/>
	</svg>
);

export default SvgPlay;
