This project uses the Libptx library (http://code.google.com/p/libptx/).

Since this project doesn't need sources of Libptx to be developed in parallel, I've decided to reference binaries rather than VS projects.
In my previous projects I used to build required binaries manually and put them under version control system.
However such approach consumes a lot of disk space (since binaries might get updated really often), so now I've come up with the alternative.
Next to this readme there's a batch file that pulls appropriate versions of Libptx and its dependencies, builds them and ilmerges them into a single library.
