const gulp = require('gulp');
const concat = require('gulp-concat');
const modify = require('gulp-modify-file');
const path = require('path');

gulp.task('default', function () {
    return gulp.src([
        './src/header.cs',
        './src/Vector2i.cs',
        './src/PriorityQueue.cs',
        './src/IPathfinder.cs',
        './src/AStarPathfinder.cs',
        './src/Model.cs',
        './src/Core.cs',
        './src/Player.cs'
    ])
        .pipe(modify((content, filePath, file) => {

            if (path.basename(filePath) !== 'header.cs')
                return content.replace(/using.*/g, "");
            else
                return content;
        }))
        .pipe(concat('Build.cs', { newLine: '\n\n' }))
        .pipe(gulp.dest('./build/'));
});