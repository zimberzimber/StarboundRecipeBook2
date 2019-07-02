/// <binding AfterBuild='_initAll' ProjectOpened='sass-watch' />
var gulp = require('gulp')
var sass = require('gulp-sass')
var concat = require('gulp-concat')
var uglify = require('gulp-uglify') // For JS
var cssMini = require('gulp-cssmin')

// Paths
var paths = {
	webroot: "./wwwroot/",
	sass: "./Sass/*.scss"
}
paths.compiledCss = paths.webroot + "styles/"

// Compile SCSS files
gulp.task("compile-sass", () => {
	return gulp.src(paths.sass)
		.pipe(sass().on('error', sass.logError))
		.pipe(concat("styles.css"))
		.pipe(cssMini())
		.pipe(gulp.dest(paths.compiledCss))
})

// Watch the SCSS file, and compile when it undergoes modifications
gulp.task("sass-watch", () => {
	gulp.watch(paths.sass, gulp.series("compile-sass"))
})

// Run all the tasks
gulp.task("_initAll", gulp.parallel("compile-sass"))